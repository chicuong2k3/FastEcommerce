using Microsoft.Extensions.Caching.Distributed;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using System.Text.Json;

namespace Ordering.Infrastructure.Persistence.Repositories;

public class CachedCartRepository : ICartRepository
{
    private readonly IDistributedCache cache;
    private readonly ICartRepository innerRepository;

    public CachedCartRepository(IDistributedCache cache, ICartRepository innerRepository)
    {
        this.cache = cache;
        this.innerRepository = innerRepository;
    }

    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public async Task<Cart?> GetAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        var cachedCart = await cache.GetStringAsync(ownerId.ToString(), cancellationToken);

        if (!string.IsNullOrEmpty(cachedCart))
        {
            return JsonSerializer.Deserialize<Cart>(cachedCart, jsonOptions);
        }

        var cart = await innerRepository.GetAsync(ownerId, cancellationToken);

        if (cart != null)
        {
            await cache.SetStringAsync(ownerId.ToString(), JsonSerializer.Serialize(cart, jsonOptions), cancellationToken);
        }

        return cart;
    }

    public async Task UpsertAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await innerRepository.UpsertAsync(cart, cancellationToken);
        await cache.SetStringAsync(cart.OwnerId.ToString(), JsonSerializer.Serialize(cart, jsonOptions), cancellationToken);
    }
}