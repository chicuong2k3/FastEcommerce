namespace Catalog.Application.Features.ProductAttributes;

public record CreateProductAttributeCommand(
    string Name,
    string? DisplayName,
    bool IsOption,
    string? Unit) : ICommand<ProductAttributeReadModel>;

internal class CreateProductAttributeCommandHandler(IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<CreateProductAttributeCommand, ProductAttributeReadModel>
{
    public async Task<Result<ProductAttributeReadModel>> Handle(CreateProductAttributeCommand command, CancellationToken cancellationToken)
    {
        var existingProductAttribute = await productAttributeRepository.GetByNameAsync(command.Name.ToLower(), cancellationToken);
        if (existingProductAttribute != null)
        {
            return Result.Fail($"Product attribute with name '{command.Name}' already exist");
        }

        var result = ProductAttribute.Create(command.Name, command.DisplayName, command.IsOption, command.Unit);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        var productAttribute = result.Value;
        await productAttributeRepository.AddAsync(productAttribute, cancellationToken);
        return Result.Ok(productAttribute.ToReadModel());
    }
}
