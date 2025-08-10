using MassTransit;
using MassTransit.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using Shared.Application;
using Shared.Contracts;
using Shared.Core;
using Shared.Infrastructure.Inbox;
using Shared.Infrastructure.MediatR;
using Shared.Infrastructure.Outbox;
using Shared.Infrastructure.Repositories;
using System.Data;
using System.Reflection;

namespace Shared.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterCommonServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRegistrationConfigurator> configureConsumers,
        Assembly coreAssembly,
        Assembly applicationAssembly,
        Assembly infrastructureAssembly,
        Type dbContextType)
    {
        // Add MediatR
        services.AddMediatR(configure =>
        {
            configure.RegisterServicesFromAssemblies([applicationAssembly]);
            configure.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            configure.AddOpenBehavior(typeof(CachePipelineBehavior<,>));
        });

        // Register Event Handlers
        services.AddDomainEventHandlers(applicationAssembly, dbContextType);
        services.AddIntegrationEventHandlers(infrastructureAssembly, dbContextType);

        // Persistence & Outbox & Inbox
        var dbConnectionString = configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("'Database' connection string cannot be null or empty.");

        services.AddSingleton<DomainEventsToOutboxMessagesInterceptor>();

        DomainEventTypeRegistry.RegisterAllDomainEvents([coreAssembly]);
        IntegrationEventTypeRegistry.RegisterAllIntegrationEvents([applicationAssembly]);

        services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));


        var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "AddDbContext"
                        && m.GetGenericArguments().Length == 1)
            .Where(m =>
            {
                var parms = m.GetParameters();
                return parms.Length == 4 &&
                       parms[0].ParameterType == typeof(IServiceCollection) &&
                       parms[1].ParameterType == typeof(Action<IServiceProvider, DbContextOptionsBuilder>) &&
                       parms[2].ParameterType == typeof(ServiceLifetime) &&
                       parms[3].ParameterType == typeof(ServiceLifetime);
            })
            .FirstOrDefault();
        if (addDbContextMethod == null)
        {
            throw new InvalidOperationException("Suitable AddDbContext overload not found.");
        }

        var genericAddDbContext = addDbContextMethod.MakeGenericMethod(dbContextType);
        Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = (sp, options) =>
        {
            var interceptor = sp.GetRequiredService<DomainEventsToOutboxMessagesInterceptor>();
            options.UseNpgsql(dbConnectionString)
                   .AddInterceptors(interceptor)

            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging();
        };
        var invokeParams = new object[]
        {
                services,
                optionsAction,
                ServiceLifetime.Scoped,
                ServiceLifetime.Singleton
        };
        genericAddDbContext.Invoke(null, invokeParams);

        // Outbox
        services.Configure<OutboxOptions>(configuration.GetSection(nameof(OutboxOptions)));
        var outboxOptions = configuration
                                .GetSection(nameof(OutboxOptions))
                                .Get<OutboxOptions>() ?? throw new ArgumentNullException(nameof(OutboxOptions));

        var outboxJobType = typeof(ProcessOutboxMessagesJob<>).MakeGenericType(dbContextType);
        var outboxJobKey = new JobKey(outboxJobType.FullName ?? $"ProcessOutboxMessagesJob_{Guid.NewGuid()}");

        services.AddQuartz(configure =>
        {
            configure.AddJob(outboxJobType, outboxJobKey, jobBuilder => jobBuilder.StoreDurably())
                    .AddTrigger(trigger => trigger.ForJob(outboxJobKey)
                        .WithSimpleSchedule(schedule => schedule
                            .WithIntervalInSeconds(outboxOptions.PollIntervalInSeconds)
                            .RepeatForever()));

        });

        services.AddScoped(outboxJobType, provider =>
        {
            var dbContext = provider.GetRequiredService(dbContextType);
            var loggerType = typeof(ILogger<>).MakeGenericType(outboxJobType);
            var logger = provider.GetRequiredService(loggerType);
            var options = provider.GetRequiredService<IOptions<OutboxOptions>>();
            return Activator.CreateInstance(
                outboxJobType,
                dbContext,
                provider,
                new[] { applicationAssembly },
                options,
                logger)
                ?? throw new InvalidOperationException($"Failed to create instance of {outboxJobType.FullName}");
        });

        // Inbox
        services.Configure<InboxOptions>(configuration.GetSection(nameof(InboxOptions)));
        var inboxOptions = configuration
                                .GetSection(nameof(InboxOptions))
                                .Get<InboxOptions>() ?? throw new ArgumentNullException(nameof(InboxOptions));

        var inboxJobType = typeof(ProcessInboxMessagesJob<>).MakeGenericType(dbContextType);
        var inboxJobKey = new JobKey(inboxJobType.FullName ?? $"ProcessInboxMessagesJob_{Guid.NewGuid()}");

        services.AddQuartz(configure =>
        {
            configure.AddJob(inboxJobType, inboxJobKey, jobBuilder => jobBuilder.StoreDurably())
                    .AddTrigger(trigger => trigger.ForJob(inboxJobKey)
                        .WithSimpleSchedule(schedule => schedule
                            .WithIntervalInSeconds(inboxOptions.PollIntervalInSeconds)
                            .RepeatForever()));

        });

        services.AddScoped(inboxJobType, provider =>
        {
            var dbContext = provider.GetRequiredService(dbContextType);
            var loggerType = typeof(ILogger<>).MakeGenericType(inboxJobType);
            var logger = provider.GetRequiredService(loggerType);
            var options = provider.GetRequiredService<IOptions<InboxOptions>>();
            return Activator.CreateInstance(
                inboxJobType,
                dbContext,
                provider,
                new[] { infrastructureAssembly },
                options,
                logger)
                ?? throw new InvalidOperationException($"Failed to create instance of {inboxJobType.FullName}");
        });

        services.AddQuartzHostedService();

        services.AddScoped<IDbConnection>(sp =>
        {
            var connection = new NpgsqlConnection(dbConnectionString);
            connection.Open();
            return connection;
        });


        // MassTransit

        services.TryAddSingleton<IEventBus, EventBus>();
        var rabbitMqOptions = new RabbitMqOptions();
        configuration.GetSection(nameof(RabbitMqOptions)).Bind(rabbitMqOptions);
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqOptions.Host, rabbitMqOptions.VHost, h =>
                {
                    h.Username(rabbitMqOptions.Username);
                    h.Password(rabbitMqOptions.Password);
                });
                cfg.ConfigureEndpoints(context);
            });

            x.SetKebabCaseEndpointNameFormatter();

            configureConsumers(x);
        });



        // Add Health Checks
        services.AddHealthChecks()
           .AddNpgSql(dbConnectionString);


        // OpenTelemetry
        services.AddOpenTelemetry()
            .ConfigureResource(res => res.AddService("Ecommerce"))
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddNpgsql()
                    .AddRedisInstrumentation()
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .AddOtlpExporter();
            });



    }

    private static void AddDomainEventHandlers(
        this IServiceCollection services,
        Assembly assembly,
        Type dbContextType)
    {
        var domainEventHandlerTypes = assembly
            .GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IDomainEventHandler)))
            .Distinct()
            .ToList();

        var handlersByEventType = domainEventHandlerTypes
            .SelectMany(handlerType => handlerType
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                .Select(i => new { HandlerType = handlerType, EventType = i.GetGenericArguments().Single() }))
            .Where(x => typeof(DomainEvent).IsAssignableFrom(x.EventType))
            .GroupBy(x => x.EventType)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.HandlerType).Distinct().ToList());

        foreach (var eventType in handlersByEventType.Keys)
        {
            var handlerTypesForEvent = handlersByEventType[eventType];

            foreach (var handlerType in handlerTypesForEvent)
            {
                var interfaceType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

                services.AddScoped(handlerType);

                services.AddScoped(interfaceType, provider =>
                {
                    var innerHandler = provider.GetRequiredService(handlerType);
                    var loggerType = typeof(ILogger<>).MakeGenericType(
                        typeof(IdempotentDomainEventHandler<,>).MakeGenericType(eventType, dbContextType));
                    var logger = provider.GetRequiredService(loggerType);
                    var idempotentHandlerType = typeof(IdempotentDomainEventHandler<,>)
                        .MakeGenericType(eventType, dbContextType);
                    try
                    {
                        return Activator.CreateInstance(idempotentHandlerType,
                            provider.GetRequiredService(dbContextType),
                            innerHandler,
                            logger)
                            ?? throw new InvalidOperationException($"Failed to create instance of {idempotentHandlerType.Name}");
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Failed to create instance of {idempotentHandlerType.Name}: {ex.Message}", ex);
                    }
                });
            }
        }

    }

    private static void AddIntegrationEventHandlers(
        this IServiceCollection services,
        Assembly assembly,
        Type dbContextType)
    {
        var integrationEventHandlerTypes = assembly
            .GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IIntegrationEventHandler)))
            .Distinct()
            .ToList();

        var handlersByEventType = integrationEventHandlerTypes
            .SelectMany(handlerType => handlerType
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>))
                .Select(i => new { HandlerType = handlerType, EventType = i.GetGenericArguments().Single() }))
            .Where(x => typeof(IntegrationEvent).IsAssignableFrom(x.EventType))
            .GroupBy(x => x.EventType)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.HandlerType).Distinct().ToList());

        foreach (var eventType in handlersByEventType.Keys)
        {
            var handlerTypesForEvent = handlersByEventType[eventType];

            foreach (var handlerType in handlerTypesForEvent)
            {
                var interfaceType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                services.AddScoped(handlerType);

                services.AddScoped(interfaceType, provider =>
                {
                    var innerHandler = provider.GetRequiredService(handlerType);
                    var loggerType = typeof(ILogger<>).MakeGenericType(
                        typeof(IdempotentIntegrationEventHandler<,>).MakeGenericType(eventType, dbContextType));
                    var logger = provider.GetRequiredService(loggerType);
                    var idempotentHandlerType = typeof(IdempotentIntegrationEventHandler<,>)
                        .MakeGenericType(eventType, dbContextType);
                    try
                    {
                        return Activator.CreateInstance(idempotentHandlerType,
                            provider.GetRequiredService(dbContextType),
                            innerHandler,
                            logger)
                            ?? throw new InvalidOperationException($"Failed to create instance of {idempotentHandlerType.Name}");
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Failed to create instance of {idempotentHandlerType.Name}: {ex.Message}", ex);
                    }
                });
            }
        }
    }

}
