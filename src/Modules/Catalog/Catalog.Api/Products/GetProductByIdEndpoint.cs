using Catalog.Application.Features.Products;

namespace Catalog.Api.Products;

public class GetProductByIdEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public GetProductByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("api/products/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new GetProductByIdQuery(id), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
