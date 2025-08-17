using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;
public class RemoveVariantEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public RemoveVariantEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("api/products/{id}/variants/{variantId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var productId = Route<Guid>("id");
        var variantId = Route<Guid>("variantId");

        var result = await _mediator.Send(new RemoveVariantCommand(productId, variantId), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
