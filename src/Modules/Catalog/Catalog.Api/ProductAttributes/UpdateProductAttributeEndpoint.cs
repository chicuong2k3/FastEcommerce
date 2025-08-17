using Catalog.Application.Features.ProductAttributes;

namespace Catalog.Api.ProductAttributes;

public class UpdateProductAttributeEndpoint : Endpoint<CreateUpdateProductAttributeRequest, ProductAttributeReadModel>
{
    private readonly IMediator _mediator;

    public UpdateProductAttributeEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/api/product-attributes/{id}");
        AllowAnonymous();
        Validator<CreateUpdateProductAttributeValidator>();
    }

    public override async Task HandleAsync(CreateUpdateProductAttributeRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new UpdateProductAttributeCommand(id, req.Name, req.DisplayName, req.IsOption, req.Unit), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
