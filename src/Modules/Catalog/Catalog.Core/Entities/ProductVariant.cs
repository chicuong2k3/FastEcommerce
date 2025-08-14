namespace Catalog.Core.Entities;

public class ProductVariant : Entity<Guid>
{
    public string? Sku { get; private set; }
    public Money BasePrice { get; private set; }
    public Money? SalePrice { get; private set; }
    public DateTimeRange? SaleEffectiveRange { get; private set; }

    private readonly List<ProductAttributeValue> _attributeValues = new();
    public IReadOnlyCollection<ProductAttributeValue> AttributeValues => _attributeValues.AsReadOnly();

    private ProductVariant() { }

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
        DateTimeRange? saleEffectiveRange)
    {
        var validationResult = ValidatePrice(basePrice, salePrice, saleEffectiveRange);
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        return Result.Ok(new ProductVariant(sku, basePrice, salePrice, saleEffectiveRange));
    }

    public Result AddAttribute(ProductAttributeValue attributeValue)
    {
        if (_attributeValues.Any(a => a.AttributeId == attributeValue.AttributeId))
            return Result.Fail(new ConflictError($"Attribute '{attributeValue.AttributeId}' already has a value assigned."));

        _attributeValues.Add(attributeValue);
        return Result.Ok();
    }

    private static Result ValidatePrice(Money basePrice, Money? salePrice, DateTimeRange? saleEffectiveRange)
    {
        var errors = new List<IError>();

        var baseValidation = basePrice.Validate();
        if (baseValidation.IsFailed)
            errors.AddRange(baseValidation.Errors);

        if (salePrice != null)
        {
            var saleValidation = salePrice.Validate();
            if (saleValidation.IsFailed)
                errors.AddRange(saleValidation.Errors);

            if (salePrice >= basePrice)
                errors.Add(new Error("Sale price must be less than base price"));
        }

        if (saleEffectiveRange != null)
        {
            var rangeValidation = saleEffectiveRange.Validate();
            if (rangeValidation.IsFailed)
                errors.AddRange(rangeValidation.Errors);

            if (salePrice == null && (saleEffectiveRange.From != null || saleEffectiveRange.To != null))
                errors.Add(new Error("Sale price cannot be null if sale from/to date is set"));
        }

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }
}
