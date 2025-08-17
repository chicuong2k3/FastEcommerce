namespace Catalog.Application.Features.Products;

public record RemoveImagesFromProductCommand(
    Guid ProductId,
    List<string> ImageUrls) : ICommand;

internal sealed class RemoveImagesFromProductCommandHandler
    (IProductRepository productRepository) : ICommandHandler<RemoveImagesFromProductCommand>
{
    public async Task<Result> Handle(RemoveImagesFromProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product == null)
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found"));

        product.RemoveImages(command.ImageUrls);
        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
