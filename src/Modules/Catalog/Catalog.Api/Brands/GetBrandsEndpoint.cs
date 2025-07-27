//using Catalog.Application.Features.Categories;

//namespace Ecommerce.Api.Catalog.Brands;

//public class GetBrandsEndpoint : Endpoint<GetCategoriesRequest, IEnumerable<BrandReadModel>>
//{
//    private readonly IMediator _mediator;

//    public GetBrandsEndpoint(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

//    public override void Configure()
//    {
//        Get("/api/brands");
//        AllowAnonymous();
//    }

//    public override async Task HandleAsync(GetCategoriesRequest req, CancellationToken ct)
//    {
//        var result = await _mediator.Send(new GetCategoriesQuery(req.Level), ct);

//        await this.ToHttpResultAsync(result, ct);

//    }
//}
