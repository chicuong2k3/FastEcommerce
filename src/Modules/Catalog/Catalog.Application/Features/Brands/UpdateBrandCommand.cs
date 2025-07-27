namespace Catalog.Application.Features.Brands;

public sealed record UpdateBrandCommand(
    Guid Id,
    string Name,
    string? Slug,
    string? PictureUrl
) : ICommand<BrandReadModel>;

internal sealed class UpdateBrandCommandHandler(
    IBrandRepository brandRepository)
    : ICommandHandler<UpdateBrandCommand, BrandReadModel>
{
    public async Task<Result<BrandReadModel>> Handle(UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        var brand = await brandRepository.GetByIdAsync(command.Id, cancellationToken);

        if (brand == null)
            return Result.Fail(new NotFoundError($"The brand with id '{command.Id}' not found"));

        var existingBrand = await brandRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingBrand != null && existingBrand.Id != command.Id)
            return Result.Fail(new ConflictError($"Brand with name '{command.Name}' already exists."));

        brand.Update(
            command.Name,
            command.Slug,
            command.PictureUrl);

        await brandRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok(brand.ToReadModel());
    }

}