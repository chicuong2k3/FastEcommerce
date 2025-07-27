using Catalog.Application.Features.Categories;

namespace Catalog.Api.Categories;

public class GetCategoryByIdEndpoint : EndpointWithoutRequest<CategoryReadModel>
{
    private readonly IMediator _mediator;

    public GetCategoryByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/categories/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new GetCategoryByIdQuery(id), ct);

        await this.ToHttpResultAsync(result, ct);
    }
}

