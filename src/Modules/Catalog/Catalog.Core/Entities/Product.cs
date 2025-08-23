using Microsoft.VisualBasic.FileIO;
using System.Text.Json;

namespace Catalog.Core.Entities;

public sealed class Product : AggregateRoot<Guid>
{
    private Product() { }

    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public Guid? BrandId { get; private set; }
    public bool IsPublished { get; private set; }
    public bool IsSimple { get; private set; }
    public SeoMeta? SeoMeta { get; private set; }
    public string? Sku { get; private set; }
    public Money? BasePrice { get; private set; }
    public Money? SalePrice { get; private set; }
    public DateTimeRange? SaleEffectiveRange { get; private set; }

    private readonly List<ProductImage> _images = new();
    private readonly List<ProductVariant> _variants = new();
    private readonly List<ProductCategory> _productCategories = new();
    private readonly List<ProductAttributeValue> _productAttributeValues = new();

    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
    public IReadOnlyCollection<ProductCategory> ProductCategories => _productCategories.AsReadOnly();
    public IReadOnlyCollection<ProductAttributeValue> ProductAttributeValues => _productAttributeValues.AsReadOnly();

    private Product(
        string name,
        string? description,
        Guid? brandId,
        string? slug,
        bool isSimple,
        List<ProductCategory> productCategories,
        string? sku,
        Money? basePrice,
        Money? salePrice,
        DateTimeRange? saleEffectiveRange,
        IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> productAttributeValuePairs)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        BrandId = brandId;
        Slug = string.IsNullOrEmpty(slug) ? SlugHelper.GenerateSlug(Name) : slug;
        IsPublished = true;
        IsSimple = isSimple;
        _productCategories = productCategories;
        Sku = sku;
        BasePrice = basePrice;
        SalePrice = salePrice;
        SaleEffectiveRange = saleEffectiveRange;
        _productAttributeValues = productAttributeValuePairs
            .Select(pair => new ProductAttributeValue(pair.ProductAttributeId, pair.ProductAttributeValue))
            .ToList();

        Raise(new ProductCreated(Id));
    }

    public static async Task<Result<Product>> CreateAsync(
        string name,
        string? description,
        Guid? brandId,
        string? slug,
        bool isSimple,
        List<Guid> categoryIds,
        string? sku,
        Money? basePrice,
        Money? salePrice,
        DateTimeRange? saleEffectiveRange,
        IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> productAttributeValuePairs,
        IProductAttributeRepository productAttributeRepository)
    {
        var errors = new List<IError>();

        errors.AddRange(ValidateProductTypeRules(isSimple, sku, basePrice, salePrice));
        errors.AddRange(ValidatePriceRules(basePrice, salePrice, saleEffectiveRange));

        var attrValidation = await ValidateProductAttributesAsync(productAttributeValuePairs, productAttributeRepository, isVariant: false);
        if (attrValidation.IsFailed)
            errors.AddRange(attrValidation.Errors);

        if (errors.Any())
            return Result.Fail(errors);

        var product = new Product(
            name,
            description,
            brandId,
            slug,
            isSimple,
            categoryIds.Select(id => new ProductCategory()
            {
                CategoryId = id
            }).ToList(),
            sku,
            basePrice,
            salePrice,
            saleEffectiveRange,
            productAttributeValuePairs);

        return product;
    }

    public void UpdateSeoMeta(
        string? metaTitle,
        string? metaDescription,
        string? metaKeywords)
    {
        SeoMeta = new SeoMeta(metaTitle, metaDescription, metaKeywords);
    }

    public async Task<Result> AddVariantAsync(
        string? sku,
        Money basePrice,
        Money? salePrice,
        DateTimeRange? saleEffectiveRange,
        IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> productAttributeValuePairs,
        IProductAttributeRepository productAttributeRepository)
    {
        if (IsSimple)
            return Result.Fail("Cannot add variant for a simple product.");

        if (!string.IsNullOrEmpty(sku) && Variants.Any(v => v.Sku == sku))
            return Result.Fail(new ConflictError("Variant with the same Sku already exists."));

        var errors = new List<IError>();
        errors.AddRange(ValidatePriceRules(basePrice, salePrice, saleEffectiveRange));

        var attrValidation = await ValidateProductAttributesAsync(productAttributeValuePairs, productAttributeRepository, isVariant: true);
        if (attrValidation.IsFailed)
            errors.AddRange(attrValidation.Errors);

        if (errors.Any())
            return Result.Fail(errors);

        var variantResult = ProductVariant.TryCreate(sku, basePrice, salePrice, saleEffectiveRange);
        if (variantResult.IsFailed)
            return Result.Fail(variantResult.Errors);

        var variant = variantResult.Value;

        foreach (var pair in productAttributeValuePairs)
        {
            var productAttributeValue = await productAttributeRepository.GetValueAsync(pair.ProductAttributeId, pair.ProductAttributeValue);

            if (productAttributeValue != null)
            {
                var addAttrResult = variant.AddAttribute(productAttributeValue);
                if (addAttrResult.IsFailed)
                    return Result.Fail(addAttrResult.Errors);

            }

        }

        if (Variants.Any(existing => AreVariantsEquivalent(existing, variant)))
            return Result.Fail(new ConflictError("Variant with the same attributes already exists."));

        _variants.Add(variant);
        Raise(new ProductVariantAdded(Id, variant.Id));
        return Result.Ok();
    }

    public Result RemoveVariant(Guid variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            return Result.Fail(new NotFoundError($"Variant with id '{variantId}' not found."));

        _variants.Remove(variant);

        Raise(new ProductVariantRemoved(Id, variantId));

        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(
        string name,
        string? description,
        Guid? brandId,
        List<Guid> categoryIds,
        string? slug,
        string? sku,
        Money? basePrice,
        Money? salePrice,
        DateTimeRange? saleEffectiveRange,
        IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> productAttributeValuePairs,
        IProductAttributeRepository productAttributeRepository
    )
    {
        var errors = new List<IError>();
        errors.AddRange(ValidateProductTypeRules(IsSimple, sku, basePrice, salePrice));
        errors.AddRange(ValidatePriceRules(basePrice, salePrice, saleEffectiveRange));

        var attrValidation = await ValidateProductAttributesAsync(productAttributeValuePairs, productAttributeRepository, isVariant: false);
        if (attrValidation.IsFailed)
            errors.AddRange(attrValidation.Errors);

        if (errors.Any())
            return Result.Fail(errors);

        Name = name;
        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
        BrandId = brandId;
        Slug = string.IsNullOrEmpty(slug) ? SlugHelper.GenerateSlug(Name) : slug;
        Sku = sku;
        BasePrice = basePrice;
        SalePrice = salePrice;
        SaleEffectiveRange = saleEffectiveRange;

        _productCategories.Clear();
        _productCategories.AddRange(categoryIds.Select(id => new ProductCategory()
        {
            CategoryId = id,
            ProductId = Id
        }));

        _productAttributeValues.Clear();
        _productAttributeValues.AddRange(productAttributeValuePairs
            .Select(pair => new ProductAttributeValue(pair.ProductAttributeId, pair.ProductAttributeValue)));

        Raise(new ProductUpdated(Id));
        return Result.Ok();
    }

    public Result AddImage(
        string imageUrl,
        string? imageAltText,
        bool isThumbnail,
        int sortOrder,
        Guid? productAttributeId,
        string? productAttributeValue)
    {
        if (IsSimple && (productAttributeId != null || productAttributeValue != null))
        {
            return Result.Fail("productAttributeId and productAttributeValue must be null when product is simple");
        }

        if (_images.Any(i => i.Url == imageUrl))
        {
            return Result.Fail(new ConflictError($"Image with URL '{imageUrl}' already exists."));
        }

        if (isThumbnail && _images.Any(i => i.IsThumbnail))
        {
            return Result.Fail(new ConflictError("A product can only have one thumbnail image."));
        }

        var image = new ProductImage(
            imageUrl,
            imageAltText,
            isThumbnail,
            sortOrder,
            productAttributeId,
            productAttributeValue
        );

        _images.Add(image);

        Raise(new ProductImageAdded(Id, image.Id));

        return Result.Ok();
    }

    public Result RemoveImages(IEnumerable<string> imageUrls)
    {
        if (imageUrls == null || !imageUrls.Any())
            return Result.Fail("No image URLs specified for removal.");

        var notFoundUrls = new List<string>();

        foreach (var url in imageUrls)
        {
            var image = _images.FirstOrDefault(i => i.Url == url);
            if (image != null)
            {
                _images.Remove(image);
                //Raise(new ProductImageRemoved(Id, image.Id));
            }
            else
            {
                notFoundUrls.Add(url);
            }
        }

        if (notFoundUrls.Any())
            return Result.Fail($"Images not found for URLs: {string.Join(", ", notFoundUrls)}");

        return Result.Ok();
    }

    private static IEnumerable<IError> ValidateProductTypeRules(bool isSimple, string? sku, Money? basePrice, Money? salePrice)
    {
        if (!isSimple && !string.IsNullOrEmpty(sku))
            yield return new Error("Sku must be null when product is not simple");

        if (!isSimple && basePrice != null)
            yield return new Error("Base price must be null when the product type is not simple");

        if (!isSimple && salePrice != null)
            yield return new Error("Sale price must be null when the product type is not simple");
    }

    private static IEnumerable<IError> ValidatePriceRules(Money? basePrice, Money? salePrice, DateTimeRange? saleEffectiveRange)
    {
        if (basePrice != null)
        {
            var baseValidation = basePrice.Validate();
            if (baseValidation.IsFailed)
            {
                foreach (var e in baseValidation.Errors)
                    yield return e;
            }

            if (salePrice != null && salePrice >= basePrice)
                yield return new Error("Sale price must be less than base price");
        }

        if (salePrice != null)
        {
            var saleValidation = salePrice.Validate();
            if (saleValidation.IsFailed)
            {
                foreach (var e in saleValidation.Errors)
                    yield return e;
            }

            if (basePrice == null)
                yield return new Error("Base price cannot be null if sale price is not null");
        }

        if (saleEffectiveRange != null)
        {
            var rangeValidation = saleEffectiveRange.Validate();
            if (rangeValidation.IsFailed)
            {
                foreach (var e in rangeValidation.Errors)
                    yield return e;
            }
        }

        if (salePrice == null && (saleEffectiveRange?.From != null || saleEffectiveRange?.To != null))
            yield return new Error("Sale price cannot be null if sale from/to is set");
    }

    private static async Task<Result> ValidateProductAttributesAsync(
        IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> pairs,
        IProductAttributeRepository repo,
        bool isVariant)
    {
        foreach (var pair in pairs)
        {
            var attr = await repo.GetByIdAsync(pair.ProductAttributeId);
            if (attr == null)
                return Result.Fail(new NotFoundError($"Product attribute with id '{pair.ProductAttributeId}' not found"));

            if (isVariant && !attr.IsOption)
                return Result.Fail($"Product attribute '{attr.Name}' is not an option and cannot be used for variants.");

            if (!isVariant && attr.IsOption)
                return Result.Fail($"Product attribute '{attr.Name}' is an option and cannot be used for products.");

        }
        return Result.Ok();
    }

    private static bool AreVariantsEquivalent(ProductVariant a, ProductVariant b) =>
        a.AttributeValues.Count() == b.AttributeValues.Count() &&
        !a.AttributeValues.Except(b.AttributeValues).Any();
}
