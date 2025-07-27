namespace Ordering.Requests;

public class RemoveItemFromCartRequest
{
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public int Quantity { get; set; }
}