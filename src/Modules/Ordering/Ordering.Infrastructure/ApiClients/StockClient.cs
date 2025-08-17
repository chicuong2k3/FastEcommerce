using InventoryService.Contracts.ApiClients;
using InventoryService.ReadModels;
using System.Net.Http.Json;

namespace Ordering.Infrastructure.ApiClients;

internal class StockClient : IStockClient
{
    private readonly HttpClient _httpClient;

    public StockClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

    public async Task<bool> ReserveStockAsync(Guid productId, Guid? productVariantId, int quantity, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/stocks/reserve",
            new
            {
                ProductId = productId,
                ProductVariantId = productVariantId,
                Quantity = quantity
            },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return true;
    }
}
