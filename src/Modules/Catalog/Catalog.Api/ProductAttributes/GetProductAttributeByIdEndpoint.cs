using Catalog.Application.Features.ProductAttributes;

namespace Catalog.Api.ProductAttributes;

public class GetProductAttributeByIdEndpoint : EndpointWithoutRequest<ProductAttributeReadModel>
{
    private readonly IMediator _mediator;

    public GetProductAttributeByIdEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/api/product-attributes/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new GetProductAttributeByIdQuery(id), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
