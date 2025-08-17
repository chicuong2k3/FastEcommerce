
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Core;

namespace NotificationService.Infrastructure;

public static class NotificationServiceModule
{
    public static void AddNotificationServiceModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));

        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

    }

}

