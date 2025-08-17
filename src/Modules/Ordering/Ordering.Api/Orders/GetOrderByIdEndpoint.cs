using Ordering.Application.Orders;
using Ordering.ReadModels;

namespace Ordering.Api.Orders;

public class GetOrderByIdEndpoint : EndpointWithoutRequest<OrderReadModel>
{
    private readonly IMediator _mediator;

    public GetOrderByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("api/orders/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var result = await _mediator.Send(new GetOrderByIdQuery(id), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
