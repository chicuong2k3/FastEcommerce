using Pricing.Core.ValueObjects;

namespace Catalog.Application.Features.Products;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? Description,
    Guid? BrandId,
    List<Guid> CategoryIds,
    string? Slug,
    string? Sku,
    decimal? BasePrice,
    decimal? SalePrice,
    DateTime? SaleFrom,
    DateTime? SaleTo) : ICommand<ProductReadModel>;

internal class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateProductCommand, ProductReadModel>
{
    public async Task<Result<ProductReadModel>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(
            command.ProductId,
            cancellationToken);

        if (product == null)
            return Result.Fail(new NotFoundError("The product not found"));

        var categories = new List<Category>();
        if (command.CategoryIds != null)
        {
            foreach (var categoryId in command.CategoryIds)
            {
                var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);

                if (category == null)
                    return Result.Fail(new NotFoundError($"The category with id '{categoryId}' not found"));

                categories.Add(category);
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


        var basePrice = command.BasePrice != null ? Money.FromDecimal(command.BasePrice.Value) : null;
        var salePrice = command.SalePrice != null ? Money.FromDecimal(command.SalePrice.Value) : null;
        var saleEffectiveRange = new DateTimeRange(command.SaleFrom, command.SaleTo);
        var price = basePrice != null ? new ProductPrice(basePrice, salePrice, saleEffectiveRange) : new(null, null, new(null, null));
        var result = product.Update(command.Name, command.Description, command.BrandId, categories, command.Slug, command.Sku, price);

        if (result.IsFailed)
            return result;

        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok(product.ToReadModel());
    }
}
