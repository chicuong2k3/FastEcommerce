


namespace Catalog.Application.Features.ProductAttributes;

public record AddValueForProductAttributeCommand(Guid ProductAttributeId,
    string AttributeValue) : ICommand;

internal sealed class AddValueForProductAttributeCommandHandler(IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<AddValueForProductAttributeCommand>
{
    public async Task<Result> Handle(AddValueForProductAttributeCommand command, CancellationToken cancellationToken)
    {
        var productAttribute = await productAttributeRepository.GetByIdAsync(command.ProductAttributeId, cancellationToken);
        if (productAttribute == null)
            return Result.Fail(new NotFoundError($"The product attribute with id '{command.ProductAttributeId}' not found"));

        var result = productAttribute.AddValue(command.AttributeValue);
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await productAttributeRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
