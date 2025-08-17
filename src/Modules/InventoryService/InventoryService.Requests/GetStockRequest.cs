namespace InventoryService.Requests;

public class GetStockRequest
{
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
}
