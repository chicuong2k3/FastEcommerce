using Ordering.Application.Features.Carts;
using Ordering.ReadModels;
using Ordering.Requests;
using Shared.Api.Exts;

namespace Ordering.Api.Carts;

public class AddItemToCartEndpoint : Endpoint<AddItemToCartRequest, CartReadModel>
{
    private readonly IMediator _mediator;

    public AddItemToCartEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("api/cart");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddItemToCartRequest req, CancellationToken ct)
    {
        var ownerId = User.GetUserId();

        var items = req.Items.Select(i => (
            i.ProductId,
            i.ProductVariantId,
            i.Quantity)).ToList();

        var result = await _mediator.Send(new AddItemToCartCommand(ownerId, items), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
