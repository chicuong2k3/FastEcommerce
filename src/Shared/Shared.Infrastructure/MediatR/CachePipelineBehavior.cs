using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Application;
using System.Text.Json;

namespace Shared.Infrastructure.MediatR;

internal class CachePipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery<TResponse>
    where TResponse : IResultBase
{
    private readonly IDistributedCache _cache;

    public CachePipelineBehavior(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = $"{typeof(TRequest).FullName}:{typeof(TResponse).FullName}";

        var cachedResponse = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            var cachedResult = JsonSerializer.Deserialize<TResponse>(cachedResponse);
            return cachedResult ?? await GetResponseAndWriteToCache(request, next, cancellationToken);
        }

        return await GetResponseAndWriteToCache(request, next, cancellationToken);
    }

    private async Task<TResponse> GetResponseAndWriteToCache(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response = await next();
        var cacheKey = $"{typeof(TRequest).FullName}:{typeof(TResponse).FullName}";
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.UtcNow.AddMinutes(5)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), cacheOptions, cancellationToken);
        return response;
    }
}
