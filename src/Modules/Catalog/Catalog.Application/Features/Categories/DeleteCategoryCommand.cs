namespace Catalog.Application.Features.Categories;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand;

internal sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository)
    : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category == null)
            return Result.Fail(new NotFoundError($"The category with id '{command.Id}' not found"));

        await categoryRepository.RemoveAsync(category, cancellationToken);
        return Result.Ok();
    }

}