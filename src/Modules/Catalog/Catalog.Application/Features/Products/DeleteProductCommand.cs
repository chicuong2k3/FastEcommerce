namespace Catalog.Application.Features.Products;

public record DeleteProductCommand(Guid Id) : ICommand;

internal sealed class DeleteProductCommandHandler(
    IProductRepository productRepository)
    : ICommandHandler<DeleteProductCommand>
{

    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product == null)
            return Result.Fail(new NotFoundError($"The product with id '{command.Id}' not found"));

        await productRepository.RemoveAsync(product, cancellationToken);
        return Result.Ok();
    }
}