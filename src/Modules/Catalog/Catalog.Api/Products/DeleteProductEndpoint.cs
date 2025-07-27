using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;

public class DeleteProductEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public DeleteProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("api/products/{id}");
        AllowAnonymous();
    }

    public async override Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new DeleteProductCommand(id), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
