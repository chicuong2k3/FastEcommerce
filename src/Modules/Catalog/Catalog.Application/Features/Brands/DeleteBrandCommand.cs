namespace Catalog.Application.Features.Brands;

public sealed record DeleteBrandCommand(Guid Id) : ICommand;

internal sealed class DeleteBrandCommandHandler(
    IBrandRepository brandRepository)
    : ICommandHandler<DeleteBrandCommand>
{
    public async Task<Result> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        var brand = await brandRepository.GetByIdAsync(command.Id, cancellationToken);

        if (brand == null)
            return Result.Fail(new NotFoundError($"The brand with id '{command.Id}' not found"));

        await brandRepository.RemoveAsync(brand, cancellationToken);
        return Result.Ok();
    }

}