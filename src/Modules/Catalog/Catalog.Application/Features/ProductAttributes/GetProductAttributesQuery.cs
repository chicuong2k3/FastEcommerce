namespace Catalog.Application.Features.ProductAttributes;

public record GetProductAttributesQuery() : IQuery<List<ProductAttributeReadModel>>;

internal class GetProductAttributesQueryHandler(IProductAttributeRepository productAttributeRepository)
    : IQueryHandler<GetProductAttributesQuery, List<ProductAttributeReadModel>>
{
    public async Task<Result<List<ProductAttributeReadModel>>> Handle(GetProductAttributesQuery request, CancellationToken cancellationToken)
    {
        var attributes = await productAttributeRepository.GetAttributesAsync(cancellationToken);
        return Result.Ok(attributes.Select(x => x.ToReadModel()).ToList());
    }
}