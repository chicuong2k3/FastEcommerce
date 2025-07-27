using InventoryService.Core.Entities;

namespace InventoryService.Core.Repositories;

public interface IStockRepository : IRepositoryBase<Stock, Guid>
{
    Task<Stock?> GetByProductIdAndVariantIdAsync(
        Guid productId,
        Guid? variantId = null,
        CancellationToken cancellationToken = default);
}
