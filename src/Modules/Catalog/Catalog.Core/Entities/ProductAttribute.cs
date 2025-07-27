using System.ComponentModel.DataAnnotations;

namespace Catalog.Core.Entities;

public class ProductAttribute : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public bool IsOption { get; private set; }
    public string? Unit { get; private set; }

    private ProductAttribute()
    {
    }

    private ProductAttribute(string name, string? displayName, bool isOption, string? unit)
    {
        Id = Guid.NewGuid();
        Name = name.ToLower();
        DisplayName = string.IsNullOrEmpty(displayName) ? Name : displayName;
        IsOption = isOption;
        Unit = unit;
    }

    public static Result<ProductAttribute> Create(string name, string? displayName, bool isOption, string? unit)
    {
        if (isOption && !string.IsNullOrEmpty(unit))
        {
            return Result.Fail("Option-based attributes should not have a unit.");
        }

        return new ProductAttribute(name, displayName, isOption, unit);
    }

    public void ChangeName(string name)
    {
        Name = name.ToLower();
    }

    public void ChangeDisplayName(string? displayName)
    {
        DisplayName = string.IsNullOrEmpty(displayName) ? Name : displayName;
    }

    public Result ChangeUnit(string? unit)
    {
        if (IsOption && !string.IsNullOrEmpty(unit))
        {
            return Result.Fail("Option-based attributes should not have a unit.");
        }

        Unit = unit;
        return Result.Ok();
    }
}