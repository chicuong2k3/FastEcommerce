using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Ordering.Contracts;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.PaymentGateways;
using PaymentService.Infrastructure.Persistence.Repositories;
using PaymentService.Core.Repositories;
using PaymentService.Core.PaymentGateways;
using Shared.Infrastructure.Inbox;

namespace PaymentService.Infrastructure;

public static class PaymentServiceModule
{
    public static void AddPaymentServiceModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Register payment gateway
        services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();
        services.AddScoped<IPaymentGateway, MomoGateway>();

        services.Configure<VNPayConfig>(configuration.GetSection("VNPayConfig"));
        services.AddScoped<IPaymentGateway, VNPayGateway>();
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        //registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<OrderPlacedForOnlinePaymentIntegrationEvent, PaymentDbContext>>();
    }

    public static IApplicationBuilder UsePayModule(this IApplicationBuilder app)
    {
        app.ApplicationServices.MigratePayDatabaseAsync().Wait();
        return app;
    }
}

