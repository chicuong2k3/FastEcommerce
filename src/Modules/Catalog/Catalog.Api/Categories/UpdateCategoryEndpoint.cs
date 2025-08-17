using Catalog.Application.Features.Categories;

namespace Catalog.Api.Categories;

public class UpdateCategoryEndpoint : Endpoint<CreateUpdateCategoryRequest, CategoryReadModel>
{
    private readonly IMediator _mediator;

    public UpdateCategoryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/categories/{id}");
        AllowAnonymous();
        Validator<CreateUpdateCategoryValidator>();
    }

    public override async Task HandleAsync(CreateUpdateCategoryRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var updateResult = await _mediator.Send(new UpdateCategoryCommand(
            id,
            req.Name,
            req.ParentCategoryId,
            req.Description,
            req.Slug,
            req.PictureUrl,
            req.MetaTitle,
            req.MetaDescription,
            req.MetaKeywords), ct);

        await this.ToHttpResultAsync(updateResult, ct);
    }
}
