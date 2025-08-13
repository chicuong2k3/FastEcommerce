using FluentValidation;

namespace Catalog.Requests;

public class CreateUpdateProductRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? BrandId { get; set; }
    public List<Guid> CategoryIds { get; set; }
    public string? Slug { get; set; }
    public string? Sku { get; set; }
    public bool IsSimple { get; set; } = true;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    public decimal? BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime? SaleFrom { get; set; }
    public DateTime? SaleTo { get; set; }

    public List<ProductAttributeValuePair> ProductAttributeValuePairs { get; set; } = [];
}

public class CreateUpdateProductRequestValidator : AbstractValidator<CreateUpdateProductRequest>
{
    public CreateUpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Vui lòng điền tên sản phẩm.")
            .MaximumLength(255).WithMessage("Tên sản phẩm không được vượt quá 255 ký tự.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        RuleFor(x => x.Slug)
            .MaximumLength(250).WithMessage("Slug không được vượt quá 250 ký tự.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").When(x => !string.IsNullOrWhiteSpace(x.Slug))
            .WithMessage("Slug chỉ chứa chữ thường, số và dấu gạch ngang.");

        RuleFor(x => x.Sku)
            .MaximumLength(100).WithMessage("SKU không được vượt quá 100 ký tự.");

        RuleFor(x => x.MetaTitle)
            .MaximumLength(100).WithMessage("Tiều đề meta không được vượt quá 100 ký tự.");

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500).WithMessage("Mô tả meta không được vượt quá 500 ký tự.");

        RuleFor(x => x.MetaKeywords)
            .MaximumLength(100).WithMessage("Các từ khóa meta không được vượt quá 100 ký tự.");

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0).When(x => x.BasePrice.HasValue)
            .WithMessage("Giá gốc phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0).When(x => x.SalePrice.HasValue)
            .WithMessage("Giá khuyến mãi phải lớn hơn hoặc bằng 0.")
            .LessThan(x => x.BasePrice ?? decimal.MaxValue).When(x => x.SalePrice.HasValue && x.BasePrice.HasValue)
            .WithMessage("Giá khuyến mãi phải nhỏ hơn giá gốc.");

        RuleFor(x => x.SaleFrom)
            .LessThan(x => x.SaleTo).When(x => x.SaleFrom.HasValue && x.SaleTo.HasValue)
            .WithMessage("Ngày bắt đầu khuyến mãi phải trước ngày kết thúc.");
    }
}