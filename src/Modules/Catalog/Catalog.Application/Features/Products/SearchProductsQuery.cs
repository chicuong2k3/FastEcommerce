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

internal sealed class SearchProductsQueryHandler(ICatalogMongoContext mongoContext)
    : IQueryHandler<SearchProductsQuery, PaginationResult<ProductReadModel>>
{
    public async Task<Result<PaginationResult<ProductReadModel>>> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
    {
        var filterBuilder = Builders<ProductReadModel>.Filter;
        var variantFilterBuilder = Builders<ProductVariantReadModel>.Filter;
        var filters = new List<FilterDefinition<ProductReadModel>>();

        if (query.CategoryId.HasValue)
        {
            filters.Add(filterBuilder.AnyEq(p => p.CategoryIds, query.CategoryId.Value));
        }

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            filters.Add(filterBuilder.Regex(p => p.Name, new BsonRegularExpression(query.SearchText, "i")));
        }

        if (query.MinPrice.HasValue || query.MaxPrice.HasValue)
        {
            var simpleProductPriceFilters = new List<FilterDefinition<ProductReadModel>>();
            var variantPriceFilters = new List<FilterDefinition<ProductVariantReadModel>>();

            if (query.MinPrice.HasValue)
            {
                simpleProductPriceFilters.Add(filterBuilder.Or(
                    filterBuilder.And(
                        filterBuilder.Ne(p => p.SalePrice, null),
                        filterBuilder.Lte(p => p.SaleFrom, DateTime.UtcNow),
                        filterBuilder.Gte(p => p.SaleTo, DateTime.UtcNow),
                        filterBuilder.Gte(p => p.SalePrice, query.MinPrice.Value)
                    ),
                    filterBuilder.Gte(p => p.BasePrice, query.MinPrice.Value)
                ));
            }
            if (query.MaxPrice.HasValue)
            {
                simpleProductPriceFilters.Add(filterBuilder.Or(
                    filterBuilder.And(
                        filterBuilder.Ne(p => p.SalePrice, null),
                        filterBuilder.Lte(p => p.SaleFrom, DateTime.UtcNow),
                        filterBuilder.Gte(p => p.SaleTo, DateTime.UtcNow),
                        filterBuilder.Lte(p => p.SalePrice, query.MaxPrice.Value)
                    ),
                    filterBuilder.Lte(p => p.BasePrice, query.MaxPrice.Value)
                ));
            }

            if (query.MinPrice.HasValue)
            {
                variantPriceFilters.Add(variantFilterBuilder.Or(
                    variantFilterBuilder.And(
                        variantFilterBuilder.Ne(v => v.SalePrice, null),
                        variantFilterBuilder.Lte(v => v.SaleFrom, DateTime.UtcNow),
                        variantFilterBuilder.Gte(v => v.SaleTo, DateTime.UtcNow),
                        variantFilterBuilder.Gte(v => v.SalePrice, query.MinPrice.Value)
                    ),
                    variantFilterBuilder.Gte(v => v.BasePrice, query.MinPrice.Value)
                ));
            }
            if (query.MaxPrice.HasValue)
            {
                variantPriceFilters.Add(variantFilterBuilder.Or(
                    variantFilterBuilder.And(
                        variantFilterBuilder.Ne(v => v.SalePrice, null),
                        variantFilterBuilder.Lte(v => v.SaleFrom, DateTime.UtcNow),
                        variantFilterBuilder.Gte(v => v.SaleTo, DateTime.UtcNow),
                        variantFilterBuilder.Lte(v => v.SalePrice, query.MaxPrice.Value)
                    ),
                    variantFilterBuilder.Lte(v => v.BasePrice, query.MaxPrice.Value)
                ));
            }

            var simpleProductPriceFilter = simpleProductPriceFilters.Any() ? filterBuilder.And(simpleProductPriceFilters) : filterBuilder.Empty;
            var variantPriceFilter = variantPriceFilters.Any() ? filterBuilder.ElemMatch(p => p.Variants, variantFilterBuilder.And(variantPriceFilters)) : filterBuilder.Empty;

            filters.Add(filterBuilder.Or(simpleProductPriceFilter, variantPriceFilter));
        }

        var filter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

        var sortBuilder = Builders<ProductReadModel>.Sort;
        SortDefinition<ProductReadModel> sort = query.SortBy?.ToLower() switch
        {
            "name" or "name asc" => sortBuilder.Ascending(p => p.Name),
            "name desc" => sortBuilder.Descending(p => p.Name),
            "price" or "price asc" => sortBuilder.Ascending(p => p.Variants[0].SalePrice != null && p.Variants[0].SaleFrom <= DateTime.UtcNow && p.Variants[0].SaleTo >= DateTime.UtcNow ? p.Variants[0].SalePrice : p.Variants[0].BasePrice),
            "price desc" => sortBuilder.Descending(p => p.Variants[0].SalePrice != null && p.Variants[0].SaleFrom <= DateTime.UtcNow && p.Variants[0].SaleTo >= DateTime.UtcNow ? p.Variants[0].SalePrice : p.Variants[0].BasePrice),
            _ => sortBuilder.Ascending(p => p.Id)
        };

        int skip = (query.PageNumber - 1) * query.PageSize;

        var productsTask = mongoContext.Products.Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(query.PageSize)
            .ToListAsync(cancellationToken);

        var totalCountTask = mongoContext.Products.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        await Task.WhenAll(productsTask, totalCountTask);

        var products = await productsTask;
        var totalCount = await totalCountTask;

        return Result.Ok(new PaginationResult<ProductReadModel>(
            query.PageNumber,
            query.PageSize,
            totalCount,
            products));

    }
}