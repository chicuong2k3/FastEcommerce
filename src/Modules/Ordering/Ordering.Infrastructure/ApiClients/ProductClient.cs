using Catalog.Contracts.ApiClients;
using Catalog.ReadModels;

namespace Ordering.Infrastructure.ApiClients;

using System.Net.Http.Json;

internal class ProductClient : IProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CatalogClient");
    }

    public async Task<ProductReadModel?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"api/products/{productId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ProductReadModel>(cancellationToken: cancellationToken);
    }
}
