using Ordering.Application.Orders;
using Ordering.ReadModels;
using Ordering.Requests;
using Shared.Api.Exts;

namespace Ordering.Api.Orders;

public class CheckoutEndpoint : Endpoint<CheckoutRequest, OrderReadModel>
{
    private readonly IMediator _mediator;

    public CheckoutEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("api/orders/checkout");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CheckoutRequest req, CancellationToken ct)
    {
        var customerId = User.GetUserId();

        var result = await _mediator.Send(new CheckoutCommand(
            customerId,
            req.Street,
            req.Ward,
            req.District,
            req.Province,
            req.Country,
            req.PhoneNumber,
            req.PaymentMethod,
            req.ShippingMethod,
            req.Tax), ct);

        await this.ToHttpResultAsync(result, ct);
    }
}
