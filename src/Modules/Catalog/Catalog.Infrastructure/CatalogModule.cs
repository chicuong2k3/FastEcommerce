using Catalog.Application;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using Ordering.Contracts;
using Shared.Infrastructure;
using Shared.Infrastructure.Inbox;

namespace Catalog.Infrastructure;

public static class CatalogModule
{
    public static void AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterCommonServices(
            configuration,
            ConfigureConsumers,
            Core.AssemblyInfo.Ref,
            Application.AssemblyInfo.Ref,
            AssemblyInfo.Ref,
            typeof(CatalogDbContext)
        );

        var mongoConnectionString = configuration.GetConnectionString("Mongo");
        var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);
        mongoClientSettings.ClusterConfigurator = c => c.Subscribe(new DiagnosticsActivityEventSubscriber
        (
            new InstrumentationOptions()
            {
                CaptureCommandText = true
            }
        ));

        services.AddHealthChecks()
           .AddMongoDb(
                clientFactory: sp => sp.GetRequiredService<IMongoClient>()
            );

        services.AddSingleton<IMongoClient>(new MongoClient(mongoClientSettings));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();

        services.AddScoped<ICatalogMongoContext, CatalogMongoContext>();

    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<OrderCreatedIntegrationEvent, CatalogDbContext>>();
        //registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<ProductDeletedIntegrationEvent, CatalogDbContext>>();
    }

}

