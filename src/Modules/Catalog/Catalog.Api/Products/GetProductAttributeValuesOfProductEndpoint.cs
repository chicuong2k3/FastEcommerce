using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;

public class GetProductAttributeValuesOfProductEndpoint
    : EndpointWithoutRequest<IEnumerable<AttributeValueReadModel>>
{
    private readonly IMediator _mediator;

    public GetProductAttributeValuesOfProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("api/products/{id}/attribute-values");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var productId = Route<Guid>("id");

        var result = await _mediator.Send(
            new GetProductAttributeValuesOfProductQuery(productId),
            ct
        );

        await this.ToHttpResultAsync(result, ct);
    }
}
