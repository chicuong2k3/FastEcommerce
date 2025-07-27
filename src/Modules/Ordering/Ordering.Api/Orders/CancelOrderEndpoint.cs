using Ordering.Application.Orders;

namespace Ordering.Api.Orders;

public class CancelOrderEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public CancelOrderEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("api/orders/{id:guid}/cancel");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var result = await _mediator.Send(new CancelOrderCommand(id), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
