namespace Catalog.Core.Entities;

public class ProductVariant : Entity<Guid>
{
    public string? Sku { get; private set; }
    public ProductPrice Price { get; private set; }
    private readonly List<ProductAttributeValue> _attributeValues = [];
    public IReadOnlyCollection<ProductAttributeValue> AttributeValues => _attributeValues.AsReadOnly();

    private ProductVariant()
    {
    }

    private ProductVariant(
        string? sku,
        ProductPrice price)
    {
        Id = Guid.NewGuid();
        Sku = sku;
        Price = price;
    }

    public static Result<ProductVariant> TryCreate(string? sku, ProductPrice price)
    {
        var validationResult = price.Validate();
        if (validationResult.IsFailed)
        {
            return validationResult;
        }

        return new ProductVariant(sku, price);
    }

    public Result AddAttribute(ProductAttributeValue attributeValue)
    {
        if (_attributeValues.Any(a => a.AttributeId == attributeValue.AttributeId))
            return Result.Fail(new ConflictError($"Attribute '{attributeValue.AttributeId}' has been assigned a value."));

        _attributeValues.Add(attributeValue);
        return Result.Ok();
    }
}
