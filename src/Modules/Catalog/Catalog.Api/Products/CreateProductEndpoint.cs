using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;

public class CreateProductEndpoint : Endpoint<CreateUpdateProductRequest, ProductReadModel>
{
    private readonly IMediator _mediator;

    public CreateProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("api/products");
        AllowAnonymous();
        Validator<CreateUpdateProductRequestValidator>();
    }

    public override async Task HandleAsync(CreateUpdateProductRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateProductCommand(
            req.Name,
            req.Description,
            req.BrandId,
            req.CategoryIds,
            req.IsSimple,
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
