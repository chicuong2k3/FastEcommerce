using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;

public class UpdateProductEndpoint : Endpoint<CreateUpdateProductRequest, ProductReadModel>
{
    private readonly IMediator _mediator;

    public UpdateProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("api/products/{id}");
        AllowAnonymous();
        Validator<CreateUpdateProductRequestValidator>();
    }

    public override async Task HandleAsync(CreateUpdateProductRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new UpdateProductCommand(
            id,
            req.Name,
            req.Description,
            req.BrandId,
            req.CategoryIds,
            req.Slug,
            req.MetaTitle,
            req.MetaDescription,
            req.MetaKeywords,
            req.Sku,
            req.BasePrice,
            req.SalePrice,
            req.SaleFrom,
            req.SaleTo,
            req.ProductAttributeValuePairs.Select(p => (p.ProductAttributeId, p.ProductAttributeValue))), ct);

        await this.ToHttpResultAsync(result, ct);
    }
}
