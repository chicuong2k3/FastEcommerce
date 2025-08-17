using Catalog.Application.Features.Categories;

namespace Catalog.Api.Categories;

public class DeleteCategoryEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public DeleteCategoryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/categories/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await _mediator.Send(new DeleteCategoryCommand(id), ct);

        await this.ToHttpResultAsync(result, ct);

    }
}
