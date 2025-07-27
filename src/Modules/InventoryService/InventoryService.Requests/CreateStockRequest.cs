namespace InventoryService.Requests;

public class CreateStockRequest
{
    public Guid ProductId { get; set; }
    public Guid? VariantId { get; set; }
    public int AvailableQuantity { get; set; }
}
