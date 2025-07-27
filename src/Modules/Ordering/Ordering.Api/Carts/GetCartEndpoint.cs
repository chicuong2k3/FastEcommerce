using Ordering.Application.Features.Carts;
using Ordering.ReadModels;
using Shared.Api.Exts;

namespace Ordering.Api.Carts;

public class GetCartEndpoint : EndpointWithoutRequest<CartReadModel>
{
    private readonly IMediator _mediator;

    public GetCartEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("api/cart");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        var result = await _mediator.Send(new GetCartQuery(ownerId), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
