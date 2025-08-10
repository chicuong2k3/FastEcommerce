using Catalog.Application.Features.ProductAttributes;

namespace Catalog.Api.ProductAttributes;

public class AddValueForProductAttributeEndpoint : Endpoint<AddValueForProductAttributeRequest>
{
    private readonly IMediator _mediator;

    public AddValueForProductAttributeEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/api/product-attributes/{id}/values");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddValueForProductAttributeRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new AddValueForProductAttributeCommand(id, req.ProductAttributeValue), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
