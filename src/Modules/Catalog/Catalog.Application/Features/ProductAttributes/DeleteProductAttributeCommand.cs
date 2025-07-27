namespace Catalog.Application.Features.ProductAttributes;

public record DeleteProductAttributeCommand(Guid Id) : ICommand;

internal class DeleteProductAttributeCommandHandler(IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<DeleteProductAttributeCommand>
{
    public async Task<Result> Handle(DeleteProductAttributeCommand command, CancellationToken cancellationToken)
    {
        var productAttribute = await productAttributeRepository.GetByIdAsync(command.Id, cancellationToken);

        if (productAttribute == null)
            return Result.Fail(new NotFoundError($"ProductAttribute with id '{command.Id}' not found"));

        await productAttributeRepository.RemoveAsync(productAttribute, cancellationToken);
        return Result.Ok();
    }
}
