using Dapper;
using System.Data;

namespace Catalog.Application.Features.Categories;

public sealed record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryReadModel>;

internal sealed class GetCategoryByIdQueryHandler(IDbConnection dbConnection)
    : IQueryHandler<GetCategoryByIdQuery, CategoryReadModel>
{
    public async Task<Result<CategoryReadModel>> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
    {
        const string categoryQuery = """
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
                c."MetaKeywords"
            FROM "catalog"."Categories" c
            WHERE c."Id" = @Id
            """
        ;

        var category = await dbConnection.QueryFirstOrDefaultAsync<CategoryReadModel>(categoryQuery, new { query.Id });

        if (category == null)
            return Result.Fail(new NotFoundError($"The category with id '{query.Id}' not found"));

        const string subCategoriesQuery = """
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
                c."MetaKeywords"
            FROM "catalog"."Categories" c
            WHERE c."ParentCategoryId" = @ParentId
            """
        ;

        var subCategories = await dbConnection.QueryAsync<CategoryReadModel>(
            subCategoriesQuery,
            new { ParentId = query.Id });

        if (subCategories.Any())
        {
            var subCategoriesList = subCategories.ToList();
            category.SubCategories = subCategoriesList;
        }

        return Result.Ok(category);
    }
}