using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;
public class AddVariantEndpoint : Endpoint<AddVariantRequest>
{
    private readonly IMediator _mediator;

    public AddVariantEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("api/products/{id}/variants");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddVariantRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new AddVariantForProductCommand(
            id,
            req.ProductAttributeValuePairs.Select(p => (p.ProductAttributeId, p.ProductAttributeValue)),
            req.Sku,
            req.BasePrice,
            req.SalePrice,
            req.SaleFrom,
            req.SaleTo), ct);

        await this.ToHttpResultAsync(result, ct);
    }
}
