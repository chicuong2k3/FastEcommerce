﻿using Catalog.Contracts.ApiClients;
using InventoryService.Contracts.ApiClients;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.ApiClients;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Persistence.Repositories;
using Shared.Core;
using Shared.Infrastructure;

namespace Ordering.Infrastructure;

public static class OrderingModule
{
    public static void AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterCommonServices(
            configuration,
            ConfigureConsumers,
            Core.AssemblyInfo.Ref,
            Application.AssemblyInfo.Ref,
            AssemblyInfo.Ref,
            typeof(OrderingDbContext)
        );

        // Caching
        var cacheConnectionString = configuration.GetConnectionString("Cache") ?? throw new ArgumentNullException("Cache Connection String is not configured.");
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cacheConnectionString;
        });

        services.AddHealthChecks()
            .AddRedis(cacheConnectionString);

        services.AddScoped<ICartRepository, CartRepository>();
        //services.Decorate<ICartRepository, CachedCartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IProductClient, ProductClient>();
        services.AddScoped<IStockClient, StockClient>();

        services.AddHttpClient("CatalogClient", client =>
        {
            client.BaseAddress = new Uri(configuration["ApiClients:Catalog"] ?? throw new ArgumentException("ApiClients:Catalog is not configured"));
        });

        services.AddHttpClient("InventoryClient", client =>
        {
            client.BaseAddress = new Uri(configuration["ApiClients:Inventory"] ?? throw new ArgumentException("ApiClients:Inventory is not configured"));
        });
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        //registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<PaymentSucceededIntegrationEvent, OrderingDbContext>>();
        //registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<PaymentFailedIntegrationEvent, OrderingDbContext>>();
    }

}
