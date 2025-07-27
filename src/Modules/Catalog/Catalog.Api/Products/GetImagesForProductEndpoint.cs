using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;

public class GetImagesForProductEndpoint : Endpoint<GetImagesForProductRequest, List<string?>>
{
    private readonly IMediator _mediator;

    public GetImagesForProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("api/products/{id}/images");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetImagesForProductRequest req, CancellationToken ct)
    {
        var productId = Route<Guid>("id");
        var result = await _mediator.Send(new GetProductImagesQuery(productId, req.ProductAttributeId, req.ProductAttributeValue), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}

