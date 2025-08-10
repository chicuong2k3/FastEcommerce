
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using UIShared.Abstractions;
using UIShared.Implementations;

namespace UIShared;

public static class RegisterServices
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {


        services.AddScoped<ResponseHandler>();
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:20001");
        });


        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductAttributeService, ProductAttributeService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUploadService, UploadService>();

        return services;
    }
}