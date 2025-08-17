namespace Catalog.Application.Features.Products;

public record AddImageForProductCommand(
    Guid ProductId,
    Guid? ProductAttributeId,
    string? ProductAttributeValue,
    string ImageUrl,
    string? ImageAltText,
    bool IsThumbnail,
    int SortOrder) : ICommand;

internal sealed class AddImageForProductCommandHandler(
    IProductRepository productRepository,
    IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<AddImageForProductCommand>
{
    public async Task<Result> Handle(AddImageForProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product == null)
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found"));

        if (command.ProductAttributeId != null)
        {
            var productAttribute = await productAttributeRepository.GetByIdAsync(command.ProductAttributeId.Value, cancellationToken);
            if (productAttribute == null)
                return Result.Fail(new NotFoundError($"The product attribute with id '{command.ProductAttributeId}' not found"));

            if (command.ProductAttributeValue != null)
            {
                var productAttributeValues = await productAttributeRepository.GetValuesAsync(productAttribute.Id, cancellationToken);
                var productAttributeValue = productAttributeValues.FirstOrDefault(x => x.Value.ToLower() == command.ProductAttributeValue.ToLower());
                if (productAttributeValue == null)
                    return Result.Fail(new NotFoundError($"The product attribute value with value '{command.ProductAttributeValue}' not found"));
            }
        }

        var result = product.AddImage(command.ImageUrl,
                                      command.ImageAltText,
                                      command.IsThumbnail,
                                      command.SortOrder,
                                      command.ProductAttributeId,
                                      command.ProductAttributeValue);
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}