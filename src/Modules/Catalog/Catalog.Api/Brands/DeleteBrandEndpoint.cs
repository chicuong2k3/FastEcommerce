using Catalog.Application.Features.Brands;

namespace Catalog.Api.Brands;

public class DeleteBrandEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public DeleteBrandEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/brands/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new DeleteBrandCommand(id), ct);

        await this.ToHttpResultAsync(result, ct);

    }
}
