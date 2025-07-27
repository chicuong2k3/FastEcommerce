namespace Catalog.Application.Features.Categories;

public sealed record CreateCategoryCommand(
    string Name,
    Guid? ParentCategoryId,
    string? Description,
    string? Slug,
    string? PictureUrl,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords)
    : ICommand<CategoryReadModel>;

internal sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository)
    : ICommandHandler<CreateCategoryCommand, CategoryReadModel>
{
    public async Task<Result<CategoryReadModel>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var existingCategory = await categoryRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingCategory != null)
            return Result.Fail(new ConflictError($"Category with name '{command.Name}' already exists."));

        var category = new Category(
            command.Name,
            command.Description,
            command.Slug,
            command.PictureUrl,
            new SeoMeta(command.MetaTitle, command.MetaDescription, command.MetaKeywords));

        if (command.ParentCategoryId != null)
        {
            var parentCategory = await categoryRepository.GetByIdAsync(command.ParentCategoryId.Value, cancellationToken);

            if (parentCategory == null)
                return Result.Fail(new NotFoundError($"Category with id '{command.ParentCategoryId.Value}' not found"));

            var addSubCatResult = await parentCategory.AddSubCategoryAsync(category, categoryRepository);
            if (addSubCatResult.IsFailed)
                return Result.Fail(addSubCatResult.Errors);
        }

        await categoryRepository.AddAsync(category, cancellationToken);

        return Result.Ok(category.ToReadModel());

    }

}