using FluentValidation;
namespace Catalog.Requests;

public class CreateUpdateProductAttributeRequest
{
    public string Name { get; set; }
    public string? DisplayName { get; set; }
    public bool IsOption { get; set; }
    public string? Unit { get; set; }
}

public class CreateUpdateProductAttributeValidator : AbstractValidator<CreateUpdateProductAttributeRequest>
{
    public CreateUpdateProductAttributeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Vui lòng điền tên thuộc tính sản phẩm")
            .MaximumLength(100).WithMessage("Tên thuộc tính sản phẩm không được vượt quá 100 ký tự");

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("Tên hiển thị không được vượt quá 100 ký tự");
    }
}