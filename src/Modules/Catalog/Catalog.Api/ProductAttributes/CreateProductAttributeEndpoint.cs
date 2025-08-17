using Catalog.Application.Features.ProductAttributes;

namespace Catalog.Api.ProductAttributes;

public class CreateProductAttributeEndpoint : Endpoint<CreateUpdateProductAttributeRequest, ProductAttributeReadModel>
{
    private readonly IMediator _mediator;

    public CreateProductAttributeEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/api/product-attributes");
        AllowAnonymous();
        Validator<CreateUpdateProductAttributeValidator>();
    }

    public override async Task HandleAsync(CreateUpdateProductAttributeRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateProductAttributeCommand(req.Name, req.DisplayName, req.IsOption, req.Unit), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
