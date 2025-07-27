using Pricing.Core.ValueObjects;

namespace Catalog.Application.Features.Products;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    Guid? BrandId,
    List<Guid> CategoryIds,
    bool IsSimple,
    string? Slug,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    string? Sku,
    decimal? BasePrice,
    decimal? SalePrice,
    DateTime? SaleFrom,
    DateTime? SaleTo,
    IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> ProductAttributeValuePairs
) : ICommand<ProductReadModel>;


internal sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<CreateProductCommand, ProductReadModel>
{
    public async Task<Result<ProductReadModel>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
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

        if (!string.IsNullOrEmpty(command.Sku))
        {
            var existingProduct = await productRepository.GetBySkuAsync(command.Sku, cancellationToken);
            if (existingProduct != null)
            {
                return Result.Fail(new Error($"The product with Sku '{command.Sku}' already exists"));
            }
        }

        var basePrice = command.BasePrice != null ? Money.FromDecimal(command.BasePrice.Value) : null;
        var salePrice = command.SalePrice != null ? Money.FromDecimal(command.SalePrice.Value) : null;
        var saleEffectiveRange = new DateTimeRange(command.SaleFrom, command.SaleTo);
        var price = new ProductPrice(basePrice, salePrice, saleEffectiveRange);
        var res = await Product.CreateAsync(
            command.Name,
            command.Description,
            command.BrandId,
            command.Slug,
            command.IsSimple,
            categories,
            command.Sku,
            price,
            command.ProductAttributeValuePairs,
            productAttributeRepository
            );

        if (res.IsFailed)
        {
            return Result.Fail(res.Errors);
        }

        await productRepository.AddAsync(res.Value, cancellationToken);
        return Result.Ok(res.Value.ToReadModel());
    }

}
