using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Persistence.Repositories;

internal class CartRepository : ICartRepository
{
    private readonly OrderingDbContext dbContext;

    public CartRepository(OrderingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Cart?> GetAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.OwnerId == ownerId, cancellationToken);
    }

    public async Task RemoveAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        dbContext.Carts.Remove(cart);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        var existingCart = await dbContext.Carts
                            .Where(c => c.Id == cart.Id)
                            .Include(c => c.Items)
                            .FirstOrDefaultAsync(cancellationToken);

        if (existingCart == null)
        {
            dbContext.Carts.Add(cart);
        }
        else
        {
            dbContext.Entry(existingCart).CurrentValues.SetValues(cart);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
