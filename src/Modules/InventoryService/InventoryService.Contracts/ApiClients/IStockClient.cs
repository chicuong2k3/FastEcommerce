using InventoryService.ReadModels;
using System.IO.Pipelines;

namespace InventoryService.Contracts.ApiClients;

public interface IStockClient
{
    Task<StockReadModel?> GetStockAsync(Guid productId, Guid? productVariantId, CancellationToken cancellationToken = default);
    Task<bool> ReserveStockAsync(Guid productId, Guid? productVariantId, int quantity, CancellationToken cancellationToken = default);
}
