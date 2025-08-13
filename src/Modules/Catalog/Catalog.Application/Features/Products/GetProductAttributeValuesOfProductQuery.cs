using Dapper;
using System.Data;

namespace Catalog.Application.Features.Products;

public record GetProductAttributeValuesOfProductQuery(Guid ProductId)
    : IQuery<IEnumerable<AttributeValueReadModel>>;


internal class GetProductAttributeValuesOfProductQueryHandler(IDbConnection dbConnection)
    : IQueryHandler<GetProductAttributeValuesOfProductQuery, IEnumerable<AttributeValueReadModel>>
{
    public async Task<Result<IEnumerable<AttributeValueReadModel>>> Handle(GetProductAttributeValuesOfProductQuery query, CancellationToken cancellationToken)
    {
        const string sqlCheckProduct = @"
            SELECT ""IsSimple""
            FROM catalog.""Products""
            WHERE ""Id"" = @ProductId
            LIMIT 1;
        ";

        var isSimple = await dbConnection.ExecuteScalarAsync<bool?>(sqlCheckProduct, new { query.ProductId });
        if (isSimple == null)
            return Result.Fail("Product not found");

        string sql;
        object param = new { query.ProductId };

        if (isSimple.Value)
        {
            sql = @"
                SELECT a.""Id"" AS AttributeId,
                       a.""Name"" AS AttributeName,
                       a.""DisplayName"" AS AttributeDisplayName,
                       av.""Id"" AS ValueId,
                       av.""Value"",
                       a.""Unit""
                FROM catalog.""ProductProductAttributeValue"" pav
                JOIN catalog.""ProductAttributeValues"" av 
                    ON pav.""ProductAttributeValuesId"" = av.""Id""
                JOIN catalog.""ProductAttributes"" a
                    ON av.""AttributeId"" = a.""Id""
                WHERE pav.""ProductId"" = @ProductId
            ";
        }
        else
        {
            sql = @"
                SELECT a.""Id"" AS AttributeId,
                       a.""Name"" AS AttributeName,
                       a.""DisplayName"" AS AttributeDisplayName,
                       av.""Id"" AS ValueId,
                       av.""Value"",
                       a.""Unit""
                FROM catalog.""ProductVariants"" v
                JOIN catalog.""ProductAttributeValueProductVariant"" pvav
                    ON v.""Id"" = pvav.""ProductVariantId""
                JOIN catalog.""ProductAttributeValues"" av 
                    ON pvav.""AttributeValuesId"" = av.""Id""
                JOIN catalog.""ProductAttributes"" a
                    ON av.""AttributeId"" = a.""Id""
                WHERE v.""ProductId"" = @ProductId
            ";
        }

        var results = await dbConnection.QueryAsync<AttributeValueReadModel>(sql, param);

        return Result.Ok(results);
    }

}