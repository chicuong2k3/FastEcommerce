using Catalog.Application;
using Catalog.Core.Repositories;
using Catalog.Core.Services;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Ordering.Contracts;
using Shared.Infrastructure;
using Shared.Infrastructure.Inbox;
using System.Data;

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

        services.AddScoped<IDbConnection>(sp =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            return new NpgsqlConnection(connectionString);
        });

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<OrderCreatedIntegrationEvent, CatalogDbContext>>();
        //registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<ProductDeletedIntegrationEvent, CatalogDbContext>>();
    }

}

