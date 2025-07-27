using Catalog.Application.Features.ProductAttributes;

namespace Catalog.Api.ProductAttributes;

public class GetAllProductAttributesEndpoint : EndpointWithoutRequest<List<ProductAttributeReadModel>>
{
    private readonly IMediator _mediator;

    public GetAllProductAttributesEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/api/product-attributes");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductAttributesQuery(), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}

