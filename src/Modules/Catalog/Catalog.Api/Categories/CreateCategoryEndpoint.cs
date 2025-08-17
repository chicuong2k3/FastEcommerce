using Catalog.Application.Features.Categories;

namespace Catalog.Api.Categories;

public class CreateCategoryEndpoint : Endpoint<CreateUpdateCategoryRequest, CategoryReadModel>
{
    private readonly IMediator _mediator;

    public CreateCategoryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/categories");
        Validator<CreateUpdateCategoryValidator>();
        AllowAnonymous();
        //Policies("IsAdmin");
    }

    public override async Task HandleAsync(CreateUpdateCategoryRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateCategoryCommand(
            req.Name,
            req.ParentCategoryId,
            req.Description,
            req.Slug,
            req.PictureUrl,
            req.MetaTitle,
            req.MetaDescription,
            req.MetaKeywords), ct);
        await this.ToHttpResultAsync(result, ct);
    }
}
