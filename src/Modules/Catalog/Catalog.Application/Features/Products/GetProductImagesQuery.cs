namespace Catalog.Application.Features.Products;

public record GetProductImagesQuery(Guid ProductId, Guid? ProductAttributeId, string? ProductAttributeValue)
    : IQuery<List<ProductImageReadModel>>;

internal class GetProductImagesQueryHandler()
    : IQueryHandler<GetProductImagesQuery, List<ProductImageReadModel>>
{
    public async Task<Result<List<ProductImageReadModel>>> Handle(GetProductImagesQuery query, CancellationToken cancellationToken)
    {


        return Result.Ok();
    }
}