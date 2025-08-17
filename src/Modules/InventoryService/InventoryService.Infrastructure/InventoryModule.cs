using Catalog.Contracts.ApiClients;
using InventoryService.Core.Repositories;
using InventoryService.Infrastructure.ApiClients;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure;

namespace InventoryService.Infrastructure;

public static class InventoryModule
{
    public static void AddInventoryModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterCommonServices(
            configuration,
            ConfigureConsumers,
            Core.AssemblyInfo.Ref,
            Application.AssemblyInfo.Ref,
            AssemblyInfo.Ref,
            typeof(InventoryDbContext)
        );

        services.AddScoped<IStockRepository, StockRepository>();

        services.AddScoped<IProductClient, ProductClient>();
        services.AddHttpClient("CatalogClient", client =>
        {
            client.BaseAddress = new Uri(configuration["ApiClients:Catalog"] ?? throw new ArgumentException("ApiClients:Catalog is not configured"));
        });
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
    }

}

