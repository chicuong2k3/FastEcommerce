using Catalog.Core.Services;

namespace Catalog.Core.Entities;

public class ProductVariant : Entity<Guid>
{
    public string? Sku { get; private set; }

    public Money? BasePrice { get; private set; }
    public Money? SalePrice { get; private set; }
    public DateTimeRange? SaleEffectiveRange { get; private set; }

    private readonly List<ProductAttributeValue> _attributeValues = [];
    public IReadOnlyCollection<ProductAttributeValue> AttributeValues => _attributeValues.AsReadOnly();

    private ProductVariant()
    {
    }

    private ProductVariant(
        string? sku,
        Money basePrice,
        Money? salePrice,
        DateTimeRange? saleEffectiveRange)
    {
        Id = Guid.NewGuid();
        Sku = sku;
        BasePrice = basePrice;
        SalePrice = salePrice;
        SaleEffectiveRange = saleEffectiveRange;
    }

    public static Result<ProductVariant> TryCreate(
        string? sku,
        Money basePrice,
        Money? salePrice,
        DateTimeRange? saleEffectiveRange,
        ProductService productService)
    {
        var validationResult = productService.ValidateProductPrice(basePrice, salePrice, saleEffectiveRange);
        if (validationResult.IsFailed)
        {
            return validationResult;
        }

        return new ProductVariant(sku, basePrice, salePrice, saleEffectiveRange);
    }

    public Result AddAttribute(ProductAttributeValue attributeValue)
    {
        if (_attributeValues.Any(a => a.AttributeId == attributeValue.AttributeId))
            return Result.Fail(new ConflictError($"Attribute '{attributeValue.AttributeId}' has been assigned a value."));

        _attributeValues.Add(attributeValue);
        return Result.Ok();
    }
}
