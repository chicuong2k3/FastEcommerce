namespace Ordering.Core.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task UpsertAsync(Cart cart, CancellationToken cancellationToken = default);
}
