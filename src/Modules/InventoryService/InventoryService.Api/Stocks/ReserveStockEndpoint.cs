using InventoryService.Application.Features.Stocks;
using InventoryService.Requests;

namespace InventoryService.Api.Stocks;

public class ReserveStockEndpoint : Endpoint<ReserveStockRequest>
{
    private readonly IMediator _mediator;

    public ReserveStockEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/stocks/reserve");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ReserveStockRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new ReserveStockCommand(
            req.ProductId,
            req.VariantId,
            req.Quantity), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
