using Catalog.Application.Features.Brands;

namespace Catalog.Api.Brands;

public class CreateBrandEndpoint : Endpoint<CreateUpdateBrandRequest, BrandReadModel>
{
    private readonly IMediator _mediator;

    public CreateBrandEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/brands");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUpdateBrandRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateBrandCommand(
            req.Name,
            req.Slug,
            req.ImageUrl), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
