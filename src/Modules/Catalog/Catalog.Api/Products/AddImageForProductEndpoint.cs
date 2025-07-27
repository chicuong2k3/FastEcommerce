using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;

public class AddImageForProductEndpoint : Endpoint<AddImageForProductRequest>
{
    private readonly IMediator _mediator;

    public AddImageForProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("api/products/{id}/images");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddImageForProductRequest req, CancellationToken ct)
    {
        var productId = Route<Guid>("id");
        var command = new AddImageForProductCommand(
            productId,
            req.ProductAttributeId,
            req.ProductAttributeValue,
            req.ImageUrl,
            req.ImageAltText,
            req.IsThumbnail,
            req.SortOrder);
        var result = await _mediator.Send(command, ct);
        await this.ToHttpResultAsync(result, ct);
    }
}

