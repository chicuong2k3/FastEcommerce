using InventoryService.Core.ValueObjects;

namespace InventoryService.Core.Entities;

public class Warehouse : AggregateRoot<Guid>
{
    private Warehouse(string name, Address address)
    {
        Name = name;
        Address = address;
    }

    public string Name { get; private set; }
    public Address Address { get; private set; }

    public static Result<Warehouse> Create(string name, Address address)
    {
        var validationResult = address.Validate();

        if (validationResult.IsFailed)
        {
            return Result.Fail(validationResult.Errors);
        }

        return new Warehouse(name, address);
    }


}
