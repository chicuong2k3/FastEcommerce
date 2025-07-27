using System.Text.Json.Serialization;

namespace Ordering.Core.Entities;

public class CartItem : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid? ProductVariantId { get; private set; }
    public string ProductName { get; private set; }
    public string Sku { get; private set; }
    public int Quantity { get; private set; }
    public string? VariantAttributes { get; private set; }
    public string? ImageUrl { get; private set; }
    public Money BasePrice { get; private set; }
    public Money? SalePrice { get; private set; }
    public DateTime AddedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CartItem()
    {

    }

    [JsonConstructor]
    private CartItem(
        Guid productId,
        Guid? productVariantId,
        string productName,
        string sku,
        int quantity,
        string? variantAttributes,
        string? imageUrl,
        Money basePrice,
        Money? salePrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductVariantId = productVariantId;
        ProductName = productName;
        Sku = sku;
        Quantity = quantity;
        VariantAttributes = variantAttributes;
        ImageUrl = imageUrl;
        BasePrice = basePrice;
        SalePrice = salePrice;
        AddedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public static Result<CartItem> Create(
        Guid productId,
        Guid? productVariantId,
        string productName,
        string sku,
        int quantity,
        string? variantAttributes,
        string? imageUrl,
        Money basePrice,
        Money? salePrice)
    {
        if (productVariantId.HasValue && string.IsNullOrWhiteSpace(variantAttributes))
            return Result.Fail(new Error("Variant attributes must be provided for products with variants."));

        var validationResult = basePrice.Validate();
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        if (salePrice != null)
        {
            validationResult = salePrice.Validate();
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
        }

        var cartItem = new CartItem(
            productId,
            productVariantId,
            productName,
            sku,
            quantity,
            variantAttributes,
            imageUrl,
            basePrice,
            salePrice);

        return Result.Ok(cartItem);
    }

    public Result IncreaseQuantity(int additionalQuantity)
    {
        if (additionalQuantity <= 0)
            return Result.Fail(new Error("Additional quantity must be greater than zero."));

        Quantity += additionalQuantity;
        UpdatedAt = DateTime.UtcNow;
        return Result.Ok();
    }

    public Result DecreaseQuantity(int reducedQuantity)
    {
        if (reducedQuantity <= 0)
            return Result.Fail(new Error("Reduced quantity must be greater than zero."));

        if (Quantity < reducedQuantity)
            return Result.Fail(new Error($"Reduced quantity cannot be greater than current quantity ({Quantity})."));

        Quantity -= reducedQuantity;
        UpdatedAt = DateTime.UtcNow;
        return Result.Ok();
    }

    public Result UpdateDetails(
        string productName,
        string? variantAttributes,
        string? imageUrl,
        Money basePrice,
        Money? salePrice)
    {
        if (ProductVariantId.HasValue && string.IsNullOrWhiteSpace(variantAttributes))
            return Result.Fail(new Error("Variant attributes must be provided for products with variants."));

        var validationResult = basePrice.Validate();
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        if (salePrice != null)
        {
            validationResult = salePrice.Validate();
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
        }

        ProductName = productName;
        VariantAttributes = variantAttributes;
        ImageUrl = imageUrl;
        BasePrice = basePrice;
        SalePrice = salePrice;
        UpdatedAt = DateTime.UtcNow;
        return Result.Ok();
    }
}
