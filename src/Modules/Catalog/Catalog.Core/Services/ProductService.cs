namespace Catalog.Core.Services;

public class ProductService
{
    public Result ValidateProductPrice(Money? basePrice, Money? salePrice, DateTimeRange? saleEffectiveRange)
    {
        Result? validationResult;

        if (salePrice != null)
        {
            validationResult = salePrice.Validate();
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            if (basePrice == null)
            {
                return Result.Fail("Base price cannot be null if sale price is not null");
            }
        }

        if (basePrice != null)
        {
            validationResult = basePrice.Validate();
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            if (salePrice != null && salePrice >= basePrice)
            {
                return Result.Fail("Sale price must be less than base price");
            }
        }

        validationResult = saleEffectiveRange.Validate();
        if (validationResult.IsFailed)
        {
            return validationResult;
        }

        if (salePrice == null && (saleEffectiveRange.From != null || saleEffectiveRange.To != null))
        {
            return Result.Fail("Sale price cannot be null if sale from or sale to is not null");
        }

        return Result.Ok();
    }
}
