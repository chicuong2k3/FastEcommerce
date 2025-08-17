using Catalog.Application.Features.Products;
using Shared.Core;

namespace Catalog.Api.Products;

public class SearchProductsEndpoint : Endpoint<SearchProductsRequest, PaginationResult<ProductReadModel>>
{
    private readonly IMediator _mediator;

    public SearchProductsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("api/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SearchProductsRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new SearchProductsQuery(
            req.PageSize,
            req.PageNumber,
            req.CategoryId,
            req.SearchText,
            req.SortBy,
            req.MinPrice,
            req.MaxPrice), ct);

        await this.ToHttpResultAsync(result, ct);
    }
}
