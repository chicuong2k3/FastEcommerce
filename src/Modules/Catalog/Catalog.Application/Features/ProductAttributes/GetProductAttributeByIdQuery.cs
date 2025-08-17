namespace Catalog.Application.Features.ProductAttributes;

public record GetProductAttributeByIdQuery(Guid Id) : IQuery<ProductAttributeReadModel>;


internal class GetProductAttributeByIdQueryHandler(IProductAttributeRepository productAttributeRepository)
    : IQueryHandler<GetProductAttributeByIdQuery, ProductAttributeReadModel>
{
    public async Task<Result<ProductAttributeReadModel>> Handle(GetProductAttributeByIdQuery query, CancellationToken cancellationToken)
    {
        var attribute = await productAttributeRepository.GetByIdAsync(query.Id, cancellationToken);

        if (attribute == null)
            return Result.Fail(new NotFoundError($"ProductAttribute with id '{query.Id}' not found"));

        return Result.Ok(attribute.ToReadModel());
    }
}
