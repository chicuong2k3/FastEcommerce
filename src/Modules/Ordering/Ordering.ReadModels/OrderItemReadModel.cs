namespace Ordering.ReadModels;

public class OrderItemReadModel
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; }
    public string? ImageUrl { get; set; }
    public string? AttributesDescription { get; set; }
}
