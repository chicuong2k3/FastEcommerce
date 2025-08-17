using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace Shared.Infrastructure.MediatR;

public class RequestLoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : IResult<object>
{
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehavior(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
                                  RequestHandlerDelegate<TResponse> next,
                                  CancellationToken cancellationToken)
    {
        var moduleName = typeof(TRequest).FullName?.Split('.').LastOrDefault();
        var requestName = typeof(TRequest).Name;

        Activity.Current?.SetTag("request.module", moduleName);
        Activity.Current?.SetTag("request.name", requestName);

        using (LogContext.PushProperty("Module", moduleName))
        {
            _logger.LogInformation("Handling request {RequestName}", requestName);

            var stopwatch = Stopwatch.StartNew();

            TResponse response = await next();

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (response.IsSuccess)
            {
                _logger.LogInformation("Request {RequestName} handled successfully in {ElapsedMilliseconds}ms", requestName, elapsedMs);
                _logger.LogInformation("Response body: {@ResponseBody}", response.Value);
            }
            else
            {
                using (LogContext.PushProperty("Errors", response.Errors))
                {
                    _logger.LogError("Request {RequestName} failed in {ElapsedMilliseconds}ms with errors", requestName, elapsedMs);
                    _logger.LogInformation("Response body: {@ResponseBody}", response.Errors);
                }
            }


            return response;
        }
    }
}
