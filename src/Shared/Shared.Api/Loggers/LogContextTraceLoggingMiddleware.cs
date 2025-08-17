using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Diagnostics;

namespace Shared.Api.Loggers;

internal class LogContextTraceLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LogContextTraceLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString();
        var requestPath = context.Request.Path;
        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("RequestPath", requestPath))
        {
            await _next.Invoke(context);
        }
    }
}

internal static class LogContextTraceLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseLogContextTraceLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LogContextTraceLoggingMiddleware>();
    }
}