namespace Catalog.Application.Features.Products;

public record RemoveVariantCommand(Guid ProductId, Guid VariantId) : ICommand;

internal class RemoveVariantCommandHandler(IProductRepository productRepository)
    : ICommandHandler<RemoveVariantCommand>
{
    public async Task<Result> Handle(RemoveVariantCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found."));
        }

        var result = product.RemoveVariant(command.VariantId);
        if (result.IsFailed)
        {
            return result;
        }

        await productRepository.SaveChangesAsync(cancellationToken);
        return result;
    }
}