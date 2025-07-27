using Pricing.Core.ValueObjects;

namespace Catalog.Core.ValueObjects;

public record ProductPrice : ValueObject
{
    private ProductPrice() { }

    public ProductPrice(
        Money? basePrice,
        Money? salePrice,
        DateTimeRange saleEffectiveRange)
    {
        BasePrice = basePrice;
        SalePrice = salePrice;
        SaleEffectiveRange = saleEffectiveRange;
    }

    public override Result Validate()
    {
        Result? validationResult;

        if (SalePrice != null)
        {
            validationResult = SalePrice.Validate();
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            if (BasePrice == null)
            {
                return Result.Fail("Base price cannot be null if sale price is not null");
            }
        }

        if (BasePrice != null)
        {
            validationResult = BasePrice.Validate();
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            if (SalePrice != null && SalePrice >= BasePrice)
            {
                return Result.Fail("Sale price must be less than base price");
            }
        }

        validationResult = SaleEffectiveRange.Validate();
        if (validationResult.IsFailed)
        {
            return validationResult;
        }

        if (SalePrice == null && (SaleEffectiveRange.From != null || SaleEffectiveRange.To != null))
        {
            return Result.Fail("Sale price cannot be null if sale from or sale to is not null");
        }

        return Result.Ok();
    }

    public Money? BasePrice { get; private set; }
    public Money? SalePrice { get; private set; }
    public DateTimeRange SaleEffectiveRange { get; private set; }
}
