using FluentValidation;

namespace Catalog.Requests;

public class CreateUpdateCategoryRequest
{
    public string Name { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public string? PictureUrl { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}

public class CreateUpdateCategoryValidator : AbstractValidator<CreateUpdateCategoryRequest>
{
    public CreateUpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Vui lòng điền tên danh mục")
            .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự");

        RuleFor(x => x.Slug)
            .MaximumLength(250).WithMessage("Slug không được vượt quá 250 ký tự")
            .Matches("^[a-z0-9-]*$").When(x => !string.IsNullOrWhiteSpace(x.Slug))
            .WithMessage("Slug chỉ được chứa chữ thường, số và dấu gạch ngang");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự");

        RuleFor(x => x.MetaTitle)
            .MaximumLength(100).WithMessage("Tiêu đề meta không được vượt quá 100 ký tự");

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500).WithMessage("Mô tả meta không được vượt quá 500 ký tự");

        RuleFor(x => x.MetaKeywords)
            .MaximumLength(100).WithMessage("Các từ khóa meta không được vượt quá 100 ký tự");
    }
}
