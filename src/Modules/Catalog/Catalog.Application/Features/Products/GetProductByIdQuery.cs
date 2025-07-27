using MongoDB.Driver;
namespace Catalog.Application.Features.Products;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductReadModel>;

internal sealed class GetProductByIdQueryHandler(ICatalogMongoContext mongoContext)
    : IQueryHandler<GetProductByIdQuery, ProductReadModel>
{
    public async Task<Result<ProductReadModel>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var filter = Builders<ProductReadModel>.Filter.Eq(p => p.Id, query.Id);
        var product = await mongoContext.Products.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"Product with Id {query.Id} not found"));
        }

        return Result.Ok(product);
    }
}