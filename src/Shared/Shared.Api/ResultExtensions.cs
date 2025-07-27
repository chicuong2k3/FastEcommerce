using FastEndpoints;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Shared.Core;

namespace Shared.Api;

public static class ResultExtensions
{
    public static async Task ToHttpResultAsync<TRequest, TResponse, TResult>(
        this Endpoint<TRequest, TResponse> endpoint,
        Result<TResult> result,
        CancellationToken ct)
        where TRequest : notnull
    {
        var response = endpoint.HttpContext.Response;
        var errors = result.Errors.Select(e => e.Message).ToArray();

        if (result.IsSuccess)
        {
            if (result.Value is null || result.Value?.Equals(default(TResult)) == true)
            {
                response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            response.StatusCode = StatusCodes.Status200OK;
            response.ContentType = "application/json";
            await response.WriteAsJsonAsync(result.Value, cancellationToken: ct);
        }
        else if (result.Errors.Any(e => e is NotFoundError))
        {
            var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "Not Found",
                Detail = string.Join("; ", errors),
                Status = 404,
                Instance = endpoint.HttpContext.Request.Path
            };
            response.StatusCode = 404;
            await response.WriteAsJsonAsync(problem, cancellationToken: ct);
        }
        else
        {
            var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "Bad Request",
                Detail = string.Join("; ", errors),
                Status = 400,
                Instance = endpoint.HttpContext.Request.Path
            };
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(problem, cancellationToken: ct);
        }
    }

    public static Task ToHttpResultAsync<TRequest, TResponse>(
        this Endpoint<TRequest, TResponse> endpoint,
        Result result,
        CancellationToken ct)
        where TRequest : notnull
    {
        if (result.IsSuccess)
        {
            endpoint.HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        }

        return endpoint.ToHttpResultAsync(result.ToResult<object>(), ct);
    }
}
