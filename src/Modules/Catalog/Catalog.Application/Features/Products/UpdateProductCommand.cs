namespace Catalog.Application.Features.Products;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? Description,
    Guid? BrandId,
    List<Guid> CategoryIds,
    string? Slug,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    string? Sku,
    decimal? BasePrice,
    decimal? SalePrice,
    DateTime? SaleFrom,
    DateTime? SaleTo,
    IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> ProductAttributeValuePairs)
        : ICommand<ProductReadModel>;

internal class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<UpdateProductCommand, ProductReadModel>
{
    public async Task<Result<ProductReadModel>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(
            command.ProductId,
            cancellationToken);

        if (product == null)
            return Result.Fail(new NotFoundError("The product not found"));

        if (command.CategoryIds != null)
        {
            foreach (var categoryId in command.CategoryIds)
            {
                var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);

                if (category == null)
                    return Result.Fail(new NotFoundError($"The category with id '{categoryId}' not found"));
            }
        }

        if (command.BrandId != null)
        {
            var brand = await productRepository.GetByIdAsync(command.BrandId.Value, cancellationToken);
            if (brand == null)
            {
                return Result.Fail(new NotFoundError($"The brand with id '{command.BrandId}' not found"));
            }
        }


        var basePrice = Money.FromDecimal(command.BasePrice);
        var salePrice = Money.FromDecimal(command.SalePrice);
        var saleEffectiveRange = command.SaleFrom != null || command.SaleTo != null ? new DateTimeRange(command.SaleFrom, command.SaleTo) : null;
        var result = await product.UpdateAsync(
            command.Name,
            command.Description,
            command.BrandId,
            command.CategoryIds ?? [],
            command.Slug,
            command.Sku,
            basePrice,
            salePrice,
            saleEffectiveRange,
            command.ProductAttributeValuePairs,
            productAttributeRepository);

        if (result.IsFailed)
            return result;

        product.UpdateSeoMeta(command.MetaTitle, command.MetaDescription, command.MetaKeywords);

        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok(product.ToReadModel());
    }
}
