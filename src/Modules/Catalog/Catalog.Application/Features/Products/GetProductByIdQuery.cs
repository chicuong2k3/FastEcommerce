using MongoDB.Driver;
namespace Catalog.Application.Features.Products;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductReadModel>;

internal sealed class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductByIdQuery, ProductReadModel>
{
    public async Task<Result<ProductReadModel>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(query.Id, cancellationToken);
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"Product with Id {query.Id} not found"));
        }

        return Result.Ok(product.ToReadModel());
    }
}