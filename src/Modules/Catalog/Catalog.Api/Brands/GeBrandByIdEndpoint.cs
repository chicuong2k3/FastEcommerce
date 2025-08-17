//using Catalog.Application.Features.Categories;

//namespace Ecommerce.Api.Catalog.Brands;

//public class GeBrandByIdEndpoint : EndpointWithoutRequest<BrandReadModel>
//{
//    private readonly IMediator _mediator;

//    public GeBrandByIdEndpoint(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

//    public override void Configure()
//    {
//        Get("/api/brands/{id}");
//        AllowAnonymous();
//    }

//    public override async Task HandleAsync(CancellationToken ct)
//    {
//        var id = Route<Guid>("id");
//        var result = await _mediator.Send(new GetCategoryByIdQuery(id), ct);

//        await this.ToHttpResultAsync(result, ct);
//    }
//}

