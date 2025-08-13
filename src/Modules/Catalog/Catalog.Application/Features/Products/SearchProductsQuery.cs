using Dapper;
using System.Data;

namespace Catalog.Application.Features.Products;

public record SearchProductsQuery(
    int PageSize,
    int PageNumber,
    Guid? CategoryId,
    string? SearchText,
    string? SortBy,
    decimal? MinPrice,
    decimal? MaxPrice) : IQuery<PaginationResult<ProductReadModel>>;

internal sealed class SearchProductsQueryHandler(IDbConnection dbConnection)
    : IQueryHandler<SearchProductsQuery, PaginationResult<ProductReadModel>>
{
    public async Task<Result<PaginationResult<ProductReadModel>>> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
    {
        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        if (query.CategoryId.HasValue)
        {
            whereClauses.Add(@"""PC"".""CategoryId"" = @CategoryId");
            parameters.Add("CategoryId", query.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            whereClauses.Add(@"(""P"".""Name"" ILIKE @SearchText OR ""P"".""Description"" ILIKE @SearchText)");
            parameters.Add("SearchText", $"%{query.SearchText}%");
        }

        if (query.MinPrice.HasValue)
        {
            whereClauses.Add(@"COALESCE(""P"".""SalePrice"", ""P"".""BasePrice"") >= @MinPrice");
            parameters.Add("MinPrice", query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            whereClauses.Add(@"COALESCE(""P"".""SalePrice"", ""P"".""BasePrice"") <= @MaxPrice");
            parameters.Add("MaxPrice", query.MaxPrice.Value);
        }

        var whereSql = whereClauses.Count > 0
            ? "WHERE " + string.Join(" AND ", whereClauses)
            : "";

        string orderBy = query.SortBy switch
        {
            "name desc" => @"ORDER BY ""P"".""Name"" DESC",
            "price asc" => @"ORDER BY COALESCE(""P"".""SalePrice"", ""P"".""BasePrice"") ASC",
            "price desc" => @"ORDER BY COALESCE(""P"".""SalePrice"", ""P"".""BasePrice"") DESC",
            _ => @"ORDER BY ""P"".""Name"" ASC"
        };

        var offset = (query.PageNumber - 1) * query.PageSize;
        parameters.Add("PageSize", query.PageSize);
        parameters.Add("Offset", offset);

        var sql = $@"
            SELECT 
                ""P"".""Id"", ""P"".""Name"", ""P"".""Description"", ""P"".""BrandId"", ""P"".""Slug"", ""P"".""Sku"", ""P"".""IsSimple"",
                ""P"".""MetaTitle"", ""P"".""MetaDescription"", ""P"".""MetaKeywords"",
                ""P"".""BasePrice"", ""P"".""SalePrice"", ""P"".""SaleFrom"", ""P"".""SaleTo"",
                ""PC"".""CategoryId"" AS CategoryId,

                ""V"".""Id"" AS VariantId, ""V"".""Sku"" AS VariantSku, 
                ""V"".""BasePrice"" AS VariantBasePrice, ""V"".""SalePrice"" AS VariantSalePrice,
                ""V"".""SaleFrom"" AS VariantSaleFrom, ""V"".""SaleTo"" AS VariantSaleTo,

                ""AV"".""Id"" AS AttributeId, ""AV"".""Value"" AS AttributeValue,
                ""A"".""Name"" AS AttributeName
            FROM ""catalog"".""Products"" AS ""P""
            LEFT JOIN ""catalog"".""ProductCategory"" AS ""PC"" ON ""P"".""Id"" = ""PC"".""ProductId""
            LEFT JOIN ""catalog"".""ProductVariants"" AS ""V"" ON ""P"".""Id"" = ""V"".""ProductId""
            LEFT JOIN ""catalog"".""ProductAttributeValueProductVariant"" AS ""AVPV"" ON ""V"".""Id"" = ""AVPV"".""ProductVariantId""
            LEFT JOIN ""catalog"".""ProductAttributeValues"" AS ""AV"" ON ""AVPV"".""AttributeValuesId"" = ""AV"".""Id""
            LEFT JOIN ""catalog"".""ProductAttributes"" AS ""A"" ON ""AV"".""AttributeId"" = ""A"".""Id""
            {whereSql}
            {orderBy}
            LIMIT @PageSize OFFSET @Offset;
        ";

        var productDict = new Dictionary<Guid, ProductReadModel>();

        await dbConnection.QueryAsync<ProductReadModel, Guid, ProductVariantReadModel, AttributeValueReadModel, ProductReadModel>(
            sql,
            (product, categoryId, variant, attribute) =>
            {
                if (!productDict.TryGetValue(product.Id, out var productEntry))
                {
                    productEntry = product;
                    productEntry.CategoryIds = new List<Guid>();
                    productEntry.Variants = new List<ProductVariantReadModel>();
                    productDict.Add(product.Id, productEntry);
                }

                if (categoryId != Guid.Empty && !productEntry.CategoryIds.Contains(categoryId))
                    productEntry.CategoryIds.Add(categoryId);

                if (variant != null && variant.Id != Guid.Empty)
                {
                    var existingVariant = productEntry.Variants.FirstOrDefault(v => v.Id == variant.Id);
                    if (existingVariant == null)
                    {
                        variant.ProductAttributeValuePairs = new List<AttributeValueReadModel>();
                        productEntry.Variants.Add(variant);
                        existingVariant = variant;
                    }

                    if (attribute != null && attribute.AttributeId != Guid.Empty)
                        existingVariant.ProductAttributeValuePairs.Add(attribute);
                }

                return productEntry;
            },
            param: parameters,
            splitOn: "CategoryId,VariantId,AttributeId"
        );

        var totalSql = $@"
            SELECT COUNT(DISTINCT ""P"".""Id"")
            FROM ""catalog"".""Products"" AS ""P""
            LEFT JOIN ""catalog"".""ProductCategory"" AS ""PC"" ON ""P"".""Id"" = ""PC"".""ProductId""
            {whereSql};
        ";
        var totalCount = await dbConnection.ExecuteScalarAsync<int>(totalSql, parameters);

        var paginationResult = new PaginationResult<ProductReadModel>(
            query.PageNumber,
            query.PageSize,
            totalCount,
            productDict.Values.ToList()
        );

        return Result.Ok(paginationResult);
    }
}
