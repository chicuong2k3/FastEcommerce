namespace Catalog.Application.Features.Categories;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    Guid? ParentCategoryId,
    string? Description,
    string? Slug,
    string? PictureUrl,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords
) : ICommand<CategoryReadModel>;

internal sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateCategoryCommand, CategoryReadModel>
{
    public async Task<Result<CategoryReadModel>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category == null)
            return Result.Fail(new NotFoundError($"The category with id '{command.Id}' not found"));

        var existingCategory = await categoryRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingCategory != null && existingCategory.Id != command.Id)
            return Result.Fail(new ConflictError($"Category with name '{command.Name}' already exists."));

        if (command.ParentCategoryId != null)
        {
            var parentCategory = await categoryRepository.GetByIdAsync(command.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
                return Result.Fail(new NotFoundError($"The category with id '{command.ParentCategoryId.Value}' not found"));

            var addSubCatResult = await parentCategory.AddSubCategoryAsync(category, categoryRepository);
            if (addSubCatResult.IsFailed)
                return Result.Fail(addSubCatResult.Errors);
        }

        category.Update(
            command.Name,
            command.Description,
            command.Slug,
            command.PictureUrl,
            new SeoMeta(command.MetaTitle, command.MetaDescription, command.MetaKeywords));

        await categoryRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok(category.ToReadModel());
    }

}