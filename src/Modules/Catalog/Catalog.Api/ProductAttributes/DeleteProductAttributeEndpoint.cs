using Catalog.Application.Features.ProductAttributes;

namespace Catalog.Api.ProductAttributes;

public class DeleteProductAttributeEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public DeleteProductAttributeEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Delete("/api/product-attributes/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new DeleteProductAttributeCommand(id), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
