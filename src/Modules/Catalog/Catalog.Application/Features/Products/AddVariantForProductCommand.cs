namespace Catalog.Application.Features.Products;

public sealed record AddVariantForProductCommand(
    Guid ProductId,
    IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> ProductAttributeValuePairs,
    string? Sku,
    decimal BasePrice,
    decimal? SalePrice,
    DateTime? SaleFrom,
    DateTime? SaleTo
) : ICommand;


internal sealed class AddVariantForProductCommandHandler(
    IProductRepository productRepository,
    IProductAttributeRepository productAttributeRepository,
    ProductService productService)
    : ICommandHandler<AddVariantForProductCommand>
{
    public async Task<Result> Handle(AddVariantForProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);

        if (product == null)
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found"));

        var basePrice = Money.FromDecimal(command.BasePrice);
        var salePrice = command.SalePrice != null ? Money.FromDecimal(command.SalePrice.Value) : null;
        var saleEffectiveRange = new DateTimeRange(command.SaleFrom, command.SaleTo);
        var addVariantResult = await product.AddVariantAsync(command.Sku,
                                                       basePrice,
                                                       salePrice,
                                                       saleEffectiveRange,
                                                       command.ProductAttributeValuePairs,
                                                       productAttributeRepository,
                                                       productService);
        if (addVariantResult.IsFailed)
            return Result.Fail(addVariantResult.Errors);

        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
