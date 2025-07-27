using InventoryService.Contracts.ApiClients;
using InventoryService.ReadModels;
using System.Net.Http.Json;

namespace Ordering.Infrastructure.ApiClients;

internal class StockClient : IStockClient
{
    private readonly HttpClient _httpClient;

    public StockClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("InventoryClient");
    }

    public async Task<StockReadModel?> GetStockAsync(Guid productId, Guid? productVariantId, CancellationToken cancellationToken = default)
    {
        var query = productVariantId.HasValue
            ? $"?productId={productId}&productVariantId={productVariantId}"
            : $"?productId={productId}";

        var response = await _httpClient.GetAsync($"/api/stocks{query}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<StockReadModel>(cancellationToken: cancellationToken);
    }
}
