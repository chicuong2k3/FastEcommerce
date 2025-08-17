using InventoryService.ReadModels;

namespace InventoryService.Application.Mapping;

internal static class StockMapping
{
    public static StockReadModel ToReadModel(this Stock stock)
    {
        return new StockReadModel()
        {
            ProductId = stock.ProductId,
            VariantId = stock.VariantId,
            AvailableQuantity = stock.AvailableQuantity,
            ReservedQuantity = stock.ReservedQuantity,
            IsActive = stock.IsActive
        };
    }
}
