using Catalog.Application.Features.ProductAttributes;

namespace Catalog.Api.ProductAttributes;

public class GetProductAttributeValuesEndpoint : EndpointWithoutRequest<List<AttributeValueReadModel>>
{
    private readonly IMediator _mediator;

    public GetProductAttributeValuesEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/api/product-attributes/{id}/values");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new GetProductAttributeValuesQuery(id), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
