

namespace Catalog.Core.Entities;

public sealed class Product : AggregateRoot<Guid>
{
    private Product()
    {

    }

    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public Guid? BrandId { get; private set; }

    public bool IsPublished { get; private set; }
    public bool IsSimple { get; private set; }
    public SeoMeta? SeoMeta { get; private set; }
    public string? Sku { get; private set; }
    public ProductPrice Price { get; private set; }

    private readonly List<ProductImage> _images = [];
    private readonly List<ProductVariant> _variants = [];
    private readonly List<Category> _categories = [];
    private readonly List<ProductAttributeValue> _productAttributeValues = [];
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
    public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();
    public IReadOnlyCollection<ProductAttributeValue> ProductAttributeValues => _productAttributeValues.AsReadOnly();

    private Product(
        string name,
        string? description,
        Guid? brandId,
        string? slug,
        bool isSimple,
        List<Category> categories,
        string? sku,
        ProductPrice price,
        IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> productAttributeValuePairs)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        BrandId = brandId;
        Slug = string.IsNullOrEmpty(slug) ? SlugHelper.GenerateSlug(Name) : slug;
        IsPublished = true;
        IsSimple = isSimple;
        _categories = categories;
        Sku = sku;
        Price = price;
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
        List<Category> categories,
        string? sku,
        ProductPrice price,
        IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> productAttributeValuePairs,
        IProductAttributeRepository productAttributeRepository)
    {
        if (!isSimple && !string.IsNullOrEmpty(sku))
        {
            return Result.Fail("Sku must be null when product is not simple");
        }

        if (price.BasePrice != null)
        {
            if (!isSimple)
            {
                return Result.Fail("Price-related fields must be null when the product type is not simple");
            }
        }

        var validationResult = price.Validate();
        if (validationResult.IsFailed)
        {
            return validationResult;
        }

        foreach (var pair in productAttributeValuePairs)
        {
            var productAttribute = await productAttributeRepository.GetByIdAsync(pair.ProductAttributeId);

            if (productAttribute == null)
                return Result.Fail(new NotFoundError($"Product attribute with id '{pair.ProductAttributeId}' not found"));

            if (productAttribute.IsOption)
            {
                return Result.Fail($"Product attribute '{productAttribute.Name}' is an option and cannot be used for products.");
            }

            var productAttributeValues = await productAttributeRepository.GetValuesAsync(pair.ProductAttributeId);
            var productAttributeValue = productAttributeValues.FirstOrDefault(x => x.Value.ToLower() == pair.ProductAttributeValue.ToLower());
            if (productAttributeValue == null)
            {
                productAttributeValue = new ProductAttributeValue(pair.ProductAttributeId, pair.ProductAttributeValue);
                await productAttributeRepository.AddValue(productAttributeValue);
            }
        }

        var product = new Product(name, description, brandId, slug, isSimple, categories, sku, price, productAttributeValuePairs);

        return product;
    }

    public async Task<Result> AddVariantAsync(
        string? sku,
        ProductPrice price,
        IEnumerable<(Guid ProductAttributeId, string ProductAttributeValue)> productAttributeValuePairs,
        IProductAttributeRepository productAttributeRepository)
    {
        if (IsSimple)
            return Result.Fail("Cannot add variant for this type of product.");

        var variant = Variants.FirstOrDefault(v => v.Sku == sku);
        if (variant != null)
        {
            return Result.Fail(new ConflictError("Variant with the same Sku already exists."));
        }

        var result = ProductVariant.TryCreate(sku, price);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        variant = result.Value;
        foreach (var pair in productAttributeValuePairs)
        {
            var productAttribute = await productAttributeRepository.GetByIdAsync(pair.ProductAttributeId);

            if (productAttribute == null)
                return Result.Fail(new NotFoundError($"Product attribute with id '{pair.ProductAttributeId}' not found"));

            if (!productAttribute.IsOption)
            {
                return Result.Fail($"Product attribute '{productAttribute.Name}' is not an option and cannot be used for variants.");
            }

            var productAttributeValues = await productAttributeRepository.GetValuesAsync(pair.ProductAttributeId);
            var productAttributeValue = productAttributeValues.FirstOrDefault(x => x.Value.ToLower() == pair.ProductAttributeValue.ToLower());
            if (productAttributeValue == null)
            {
                productAttributeValue = new ProductAttributeValue(pair.ProductAttributeId, pair.ProductAttributeValue);
                await productAttributeRepository.AddValue(productAttributeValue);
            }

            var addAttributeResult = variant.AddAttribute(productAttributeValue);
            if (addAttributeResult.IsFailed)
                return Result.Fail(addAttributeResult.Errors);
        }

        var isDuplicate = Variants.Any(existing => existing.AttributeValues.Count() > 0 &&
            existing.AttributeValues.Count() == variant.AttributeValues.Count() &&
            !existing.AttributeValues.Except(variant.AttributeValues).Any()
        );

        if (isDuplicate)
        {
            return Result.Fail(new ConflictError("Variant with the same attributes already exists."));
        }

        _variants.Add(variant);

        Raise(new ProductVariantAdded(Id, variant.Id));

        return Result.Ok();
    }

    public Result RemoveVariant(Guid variantId)
    {
        var variant = Variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            return Result.Fail(new NotFoundError($"Variant with id '{variantId}' not found."));

        _variants.Remove(variant);

        Raise(new ProductVariantRemoved(Id, variantId));

        return Result.Ok();
    }

    public Result Update(
        string name,
        string? description,
        Guid? brandId,
        List<Category> categories,
        string? slug,
        string? sku,
        ProductPrice price
    )
    {
        if (!IsSimple && !string.IsNullOrEmpty(sku))
        {
            return Result.Fail("Sku must be null when product is not simple");
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }

        if (BrandId != brandId)
        {
            BrandId = brandId;
        }

        if (price != null)
        {
            if (!IsSimple)
            {
                return Result.Fail("Base price must be null when product is not simple");
            }

            var validationResult = price.Validate();
            if (validationResult.IsFailed)
            {
                return validationResult;
            }
        }

        Name = name;
        Slug = string.IsNullOrEmpty(slug) ? SlugHelper.GenerateSlug(Name) : slug;
        Sku = sku;
        Price = price;
        _categories.Clear();
        _categories.AddRange(categories);

        Raise(new ProductUpdated(Id));
        return Result.Ok();
    }

    public Result AddImage(string imageUrl,
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

        var image = new ProductImage(
            imageUrl,
            imageAltText,
            isThumbnail,
            sortOrder,
            productAttributeId,
            productAttributeValue);

        _images.Add(image);
        return Result.Ok();
    }

    public void RemoveImages(List<string> imageUrls)
    {
        _images.RemoveAll(i => imageUrls.Contains(i.Url));
    }

    public void RaiseProductDeletedEvent()
    {
        Raise(new ProductDeleted(Id, Images.Select(i => i.Url).ToList()));
    }
}
