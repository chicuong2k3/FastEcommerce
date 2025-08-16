using Microsoft.Extensions.Logging;
using System.Text.Json;

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
    ILogger<AddVariantForProductCommandHandler> logger)
    : ICommandHandler<AddVariantForProductCommand>
{
    public async Task<Result> Handle(AddVariantForProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);

        if (product == null)
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found"));

        var basePrice = Money.FromDecimal(command.BasePrice);
        var salePrice = command.SalePrice != null ? Money.FromDecimal(command.SalePrice.Value) : null;
        var saleEffectiveRange = command.SaleFrom != null || command.SaleTo != null ? new DateTimeRange(command.SaleFrom, command.SaleTo) : null;

        var addVariantResult = await product.AddVariantAsync(command.Sku,
                                                       basePrice,
                                                       salePrice,
                                                       saleEffectiveRange,
                                                       command.ProductAttributeValuePairs,
                                                       productAttributeRepository);
        if (addVariantResult.IsFailed)
        {
            logger.LogError("Failed to add variant for product {ProductId}: {Errors}",
                command.ProductId, JsonSerializer.Serialize(addVariantResult.Errors));
            return Result.Fail(addVariantResult.Errors);
        }

        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
