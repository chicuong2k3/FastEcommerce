using Administration.Client.Services;
using FluentValidation;
using Radzen;

namespace Administration.Client;

public static class ConfigureSharedServices
{
    public static void AddSharedServices(this IServiceCollection services)
    {
        services.AddRadzenComponents();
        services.AddValidatorsFromAssembly(Catalog.Requests.AssemblyInfo.Ref);

        services.AddScoped<INotifService, NotifService>();
    }
}
