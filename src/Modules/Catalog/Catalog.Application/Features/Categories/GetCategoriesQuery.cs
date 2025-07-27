using Dapper;
using System.Data;

namespace Catalog.Application.Features.Categories;

public record GetCategoriesQuery(int? Level) : IQuery<IEnumerable<CategoryReadModel>>;

internal sealed class GetCategoriesQueryHandler(IDbConnection dbConnection)
    : IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryReadModel>>
{
    public async Task<Result<IEnumerable<CategoryReadModel>>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        const string sql = """
            WITH RECURSIVE CategoryHierarchy AS (
                -- Base case: Level 1 categories (no parent)
                SELECT 
                    c."Id", 
                    c."Name", 
                    c."ParentCategoryId",
                    c."Slug",
                    c."Description",
                    c."IsPublished",
                    c."PictureUrl",
                    c."MetaTitle",
                    c."MetaDescription",
                    c."MetaKeywords",
                    1 AS "Level"
                FROM "catalog"."Categories" c
                WHERE c."ParentCategoryId" IS NULL
                
                UNION ALL
                
                -- Recursive case: Child categories
                SELECT 
                    c."Id", 
                    c."Name", 
                    c."ParentCategoryId",
                    c."Slug",
                    c."Description",
                    c."IsPublished",
                    c."PictureUrl",
                    c."MetaTitle",
                    c."MetaDescription",
                    c."MetaKeywords",
                    ch."Level" + 1
                FROM "catalog"."Categories" c
                INNER JOIN CategoryHierarchy ch ON c."ParentCategoryId" = ch."Id"
            )
            SELECT 
                "Id",
                "Name",
                "ParentCategoryId"
            FROM CategoryHierarchy
            WHERE @Level IS NULL OR "Level" = @Level
            ORDER BY "Name"
            """;

        var categories = await dbConnection.QueryAsync<CategoryReadModel>(
            sql,
            new { query.Level }
        );

        return Result.Ok(categories);
    }
}
