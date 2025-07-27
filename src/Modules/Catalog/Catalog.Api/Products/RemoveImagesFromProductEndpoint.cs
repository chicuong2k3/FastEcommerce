using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;

public class RemoveImagesFromProductEndpoint : Endpoint<RemoveImagesFromProductRequest>
{
    private readonly IMediator _mediator;
    public RemoveImagesFromProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("api/products/{id}/images");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RemoveImagesFromProductRequest req, CancellationToken ct)
    {
        var productId = Route<Guid>("id");
        var command = new RemoveImagesFromProductCommand(productId, req.ImageUrls);
        var result = await _mediator.Send(command, ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
