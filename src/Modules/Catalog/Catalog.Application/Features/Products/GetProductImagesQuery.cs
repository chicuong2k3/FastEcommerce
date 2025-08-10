using MongoDB.Driver;

namespace Catalog.Application.Features.Products;

public record GetProductImagesQuery(Guid ProductId, Guid? ProductAttributeId, string? ProductAttributeValue)
    : IQuery<List<ProductImageReadModel>>;

internal class GetProductImagesQueryHandler(ICatalogMongoContext mongoContext)
    : IQueryHandler<GetProductImagesQuery, List<ProductImageReadModel>>
{
    public async Task<Result<List<ProductImageReadModel>>> Handle(GetProductImagesQuery query, CancellationToken cancellationToken)
    {
        var filter = Builders<ProductImageReadModel>.Filter.Eq(i => i.ProductId, query.ProductId);
        var productImages = await mongoContext.ProductImages.Find(filter).ToListAsync(cancellationToken);
        if (productImages == null || !productImages.Any())
        {
            return Result.Fail("No images found for the specified product.");
        }

        return Result.Ok(productImages);
    }
}