using InventoryService.Application.Features.Stocks;
using InventoryService.ReadModels;
using InventoryService.Requests;

namespace InventoryService.Api.Stocks;

public class GetStockEndpoint : Endpoint<GetStockRequest, StockReadModel>
{
    private readonly IMediator _mediator;

    public GetStockEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/stocks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetStockRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetStockQuery(req.ProductId, req.ProductVariantId), ct);

        await this.ToHttpResultAsync(result, ct);
    }
}


