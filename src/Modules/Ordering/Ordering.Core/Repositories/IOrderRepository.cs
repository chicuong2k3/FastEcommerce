namespace Ordering.Core.Repositories;

public interface IOrderRepository : IRepositoryBase<Order, Guid>
{
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
