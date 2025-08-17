namespace Ordering.ReadModels;

public class CartItemReadModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public string? ProductName { get; set; }
    public string Sku { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public string? VariantAttributes { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}