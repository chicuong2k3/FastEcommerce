namespace InventoryService.Requests;

public class ReserveStockRequest
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }
}
