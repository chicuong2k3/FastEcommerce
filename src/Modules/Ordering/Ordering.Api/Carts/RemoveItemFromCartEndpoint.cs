using Ordering.Application.Features.Carts;
using Ordering.ReadModels;
using Ordering.Requests;
using Shared.Api.Exts;


namespace Ordering.Api.Carts;

public class RemoveItemFromCartEndpoint : Endpoint<RemoveItemFromCartRequest, CartReadModel>
{
    private readonly IMediator _mediator;

    public RemoveItemFromCartEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("api/cart");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RemoveItemFromCartRequest req, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        var result = await _mediator.Send(new RemoveItemFromCartCommand(ownerId, req.ProductId, req.ProductVariantId, req.Quantity), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
