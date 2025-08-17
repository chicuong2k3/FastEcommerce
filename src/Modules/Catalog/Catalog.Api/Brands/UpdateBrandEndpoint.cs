using Catalog.Application.Features.Brands;

namespace Catalog.Api.Brands;

public class UpdateBrandEndpoint : Endpoint<CreateUpdateBrandRequest, BrandReadModel>
{
    private readonly IMediator _mediator;

    public UpdateBrandEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/brands/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUpdateBrandRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var updateResult = await _mediator.Send(new UpdateBrandCommand(
            id,
            req.Name,
            req.Slug,
            req.ImageUrl), ct);

        await this.ToHttpResultAsync(updateResult, ct);
    }
}
