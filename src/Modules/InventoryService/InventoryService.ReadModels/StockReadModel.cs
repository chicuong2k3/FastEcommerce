namespace InventoryService.ReadModels;

public class StockReadModel
{
    public Guid ProductId { get; set; }
    public Guid? VariantId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public bool IsActive { get; set; }
}
