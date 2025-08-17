using InventoryService.Application.Features.Stocks;
using InventoryService.Requests;

namespace InventoryService.Api.Stocks;

public class CreateStockEndpoint : Endpoint<CreateStockRequest>
{
    private readonly IMediator _mediator;

    public CreateStockEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/stocks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateStockRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateStockCommand(
            req.ProductId,
            req.VariantId,
            req.AvailableQuantity), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
