namespace Catalog.Application.Features.ProductAttributes;

public record GetProductAttributeValuesQuery(Guid AttributeId)
    : IQuery<List<AttributeValueReadModel>>;

internal class GetProductAttributeValuesQueryHandler(IProductAttributeRepository productAttributeRepository)
    : IQueryHandler<GetProductAttributeValuesQuery, List<AttributeValueReadModel>>
{
    public async Task<Result<List<AttributeValueReadModel>>> Handle(GetProductAttributeValuesQuery query, CancellationToken cancellationToken)
    {
        var attribute = await productAttributeRepository.GetByIdAsync(query.AttributeId, cancellationToken);
        if (attribute == null)
        {
            return Result.Fail(new NotFoundError($"Product attribute with id '{query.AttributeId}' not found."));
        }

        var values = await productAttributeRepository.GetValuesAsync(query.AttributeId, cancellationToken);

        return Result.Ok(values.Select(x => x.ToReadModel()).ToList());
    }
}
