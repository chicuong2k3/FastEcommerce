using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Application.Features.Products;

public record SearchProductsQuery(
    int PageSize,
    int PageNumber,
    Guid? CategoryId,
    string? SearchText,
    string? SortBy,
    decimal? MinPrice,
    decimal? MaxPrice) : IQuery<PaginationResult<ProductReadModel>>;

internal sealed class SearchProductsQueryHandler()
    : IQueryHandler<SearchProductsQuery, PaginationResult<ProductReadModel>>
{
    public async Task<Result<PaginationResult<ProductReadModel>>> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
    {

        return Result.Ok();

    }
}