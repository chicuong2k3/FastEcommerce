﻿namespace Catalog.Application.Features.ProductAttributes;

public record UpdateProductAttributeCommand(Guid Id,
                                            string NewName,
                                            string? NewDisplayName,
                                            string? Unit) : ICommand<ProductAttributeReadModel>;

internal class UpdateProductAttributeCommandHandler(IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<UpdateProductAttributeCommand, ProductAttributeReadModel>
{
    public async Task<Result<ProductAttributeReadModel>> Handle(UpdateProductAttributeCommand command, CancellationToken cancellationToken)
    {
        var productAttribute = await productAttributeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (productAttribute == null)
            return Result.Fail(new NotFoundError($"Product attribute with id '{command.Id}' not found"));

        var existingAttribute = await productAttributeRepository.GetByNameAsync(command.NewName, cancellationToken);
        if (existingAttribute != null && existingAttribute.Id != command.Id)
            return Result.Fail(new ConflictError($"Product attribute with name '{command.NewName}' already exists."));

        productAttribute.ChangeName(command.NewName);
        productAttribute.ChangeDisplayName(command.NewDisplayName);
        productAttribute.ChangeUnit(command.Unit);

        await productAttributeRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok(productAttribute.ToReadModel());
    }
}