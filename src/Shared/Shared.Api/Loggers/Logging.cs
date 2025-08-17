using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

namespace Shared.Api.Loggers;

public static class Logging
{
    public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger
    {
        get
        {
            return (context, loggerConfiguration) =>
            {
                var environment = context.HostingEnvironment;

                loggerConfiguration
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Application", environment.ApplicationName)
                    .Enrich.WithProperty("Environment", environment.EnvironmentName)
                    .Enrich.WithExceptionDetails()
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .WriteTo.Console();



                var elasticSearchUri = context.Configuration["Elasticsearch:Uri"]
                                            ?? throw new ArgumentNullException(
                                                "Elasticsearch:Uri",
                                                "Elasticsearch URI must be configured."
                                            );
                loggerConfiguration
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchUri))
                    {
                        IndexFormat = $"applogs-{environment.ApplicationName?.ToLower().Replace(".", "-")}-{environment.EnvironmentName.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        NumberOfShards = 2,
                        NumberOfReplicas = 1,
                    });

                if (environment.IsDevelopment())
                {
                    loggerConfiguration.MinimumLevel.Override("Ecommerce.Api", LogEventLevel.Debug);
                }


            };
        }
    }
}
