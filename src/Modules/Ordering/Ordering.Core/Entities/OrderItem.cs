

namespace Ordering.Core.Entities;

public class OrderItem : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid? ProductVariantId { get; private set; }
    public Money BasePrice { get; private set; }
    public Money? SalePrice { get; private set; }
    public int Quantity { get; private set; }
    public string ProductName { get; private set; }
    public string? ImageUrl { get; }
    public string? AttributesDescription { get; set; }

    private OrderItem()
    {
    }

    public OrderItem(
        Guid productId,
        Guid? productVariantId,
        string productName,
        int quantity,
        Money basePrice,
        Money? salePrice,
        string? imageUrl,
        string? attributesDescription)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductVariantId = productVariantId;
        ProductName = productName;
        Quantity = quantity;
        BasePrice = basePrice;
        SalePrice = salePrice;
        ImageUrl = imageUrl;
        AttributesDescription = attributesDescription;
    }
}