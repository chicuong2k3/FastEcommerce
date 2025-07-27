namespace Catalog.Application.Features.Brands;

public sealed record CreateBrandCommand(
    string Name,
    string? Slug,
    string? PictureUrl)
    : ICommand<BrandReadModel>;

internal sealed class CreateBrandCommandHandler(
    IBrandRepository brandRepository)
    : ICommandHandler<CreateBrandCommand, BrandReadModel>
{
    public async Task<Result<BrandReadModel>> Handle(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var existingBrand = await brandRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingBrand != null)
            return Result.Fail(new ConflictError($"Brand with name '{command.Name}' already exists."));

        var brand = new Brand(
            command.Name,
            command.Slug,
            command.PictureUrl);

        await brandRepository.AddAsync(brand, cancellationToken);

        return Result.Ok(brand.ToReadModel());

    }

}