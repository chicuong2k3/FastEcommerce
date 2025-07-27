using Catalog.Application.Features.Categories;

namespace Catalog.Api.Categories;

public class GetCategoriesEndpoint : Endpoint<GetCategoriesRequest, IEnumerable<CategoryReadModel>>
{
    private readonly IMediator _mediator;

    public GetCategoriesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/categories");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetCategoriesRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoriesQuery(req.Level), ct);

        await this.ToHttpResultAsync(result, ct);

    }
}
