using FluentValidation;

namespace Catalog.Requests;

public class ProductAttributeValuePair
{
    public Guid ProductAttributeId { get; set; }
    public string ProductAttributeValue { get; set; }
}

public class AddVariantRequest
{
    public List<ProductAttributeValuePair> ProductAttributeValuePairs { get; set; } = [];
    public string? Sku { get; set; }

    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime? SaleFrom { get; set; }
    public DateTime? SaleTo { get; set; }
}

public class AddVariantRequestValidator : AbstractValidator<AddVariantRequest>
{
    public AddVariantRequestValidator()
    {
        RuleFor(x => x.ProductAttributeValuePairs)
            .NotNull().WithMessage("At least one product attribute must be provided.")
            .Must(x => x.Any()).WithMessage("At least one product attribute must be provided.");

        RuleFor(x => x.Sku)
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Sku));

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("Base price must be greater than zero.");

        RuleFor(x => x.SalePrice)
            .GreaterThan(0).WithMessage("Sale price must be greater than zero.")
            .LessThan(x => x.BasePrice).WithMessage("Sale price must be less than base price.")
            .When(x => x.SalePrice.HasValue);

        RuleFor(x => x.SaleFrom)
            .LessThan(x => x.SaleTo).WithMessage("SaleFrom must be before SaleTo.")
            .When(x => x.SaleFrom.HasValue && x.SaleTo.HasValue);
    }
}