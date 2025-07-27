using InventoryService.ReadModels;

namespace InventoryService.Contracts.ApiClients;

public interface IStockClient
{
    Task<StockReadModel?> GetStockAsync(Guid productId, Guid? productVariantId, CancellationToken cancellationToken = default);
}
