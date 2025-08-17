using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.Api.Loggers;
using System.Reflection;

namespace Shared.Api;

public static class ConfigureHosting
{
    public static IServiceCollection AddSharedApi(
       this WebApplicationBuilder builder,
       IConfiguration configuration
    )
    {
        builder.Host.UseSerilog(Logging.ConfigureLogger);

        builder.Services
            .AddFastEndpoints()
            .AddEndpointsApiExplorer()
            .SwaggerDocument(o =>
            {
                o.AutoTagPathSegmentIndex = 2;
                o.DocumentSettings = s =>
                {
                    var serviceName = Assembly.GetEntryAssembly()?.GetName().Name?.Replace(".Api", "") ?? "Unknown";
                    s.Title = $"{serviceName}";

                    //var securityScheme = new OpenApiSecurityScheme
                    //{
                    //    Name = "Auth",
                    //    In = ParameterLocation.Header,
                    //    Type = SecuritySchemeType.OpenIdConnect,
                    //    OpenIdConnectUrl = new Uri(""),
                    //    Scheme = "bearer",
                    //    BearerFormat = "JWT",
                    //    Reference = new OpenApiReference
                    //    {
                    //        Id = "Bearer",
                    //        Type = ReferenceType.SecurityScheme
                    //    },
                    //    Flows = new OpenApiOAuthFlows
                    //    {
                    //        Implicit = new OpenApiOAuthFlow
                    //        {
                    //        }
                    //    }
                    //};
                };
            });


        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });


        return builder.Services;
    }

    public static IApplicationBuilder UseSharedApi(
        this IApplicationBuilder app,
        IConfiguration configuration
    )
    {
        app.UseCors();


        app.UseSerilogRequestLogging();

        //app.MapHealthChecks("/health", new HealthCheckOptions()
        //{
        //    Predicate = _ => true,
        //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        //});

        app.UseLogContextTraceLogging();

        app.UseFastEndpoints()
            .UseSwaggerGen();

        return app;
    }

}
