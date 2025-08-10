
using FluentValidation;

namespace Catalog.Requests;

public class AddValueForProductAttributeRequest
{
    public string ProductAttributeValue { get; set; } = string.Empty;
}

public class AddValueForProductAttributeRequestValidator : AbstractValidator<AddValueForProductAttributeRequest>
{
    public AddValueForProductAttributeRequestValidator()
    {
        RuleFor(x => x.ProductAttributeValue)
            .NotEmpty().WithMessage("Vui lòng điền giá trị")
            .MaximumLength(100).WithMessage("Giá trị thuộc tính không được quá 100 kí tự");
    }
}
