using System.Net.Http.Json;
using System.Text;
using NbpApp.NbpApiClient.Contracts;

namespace NbpApp.NbpApiClient;

public interface INbpApiClient
{
    public Task<NpbPriceDto[]> GetGoldPricesAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}

internal class NbpApiClient : INbpApiClient
{
    private readonly HttpClient _client;

    public NbpApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<NpbPriceDto[]> GetGoldPricesAsync(DateOnly startDate, DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        var requestUrl = $"cenyzlota/{startDate:yyyy-MM-dd}/{endDate:yyyy-MM-dd}";

        var response = await _client.GetAsync(requestUrl, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Failed to get gold prices. Status code: {response.StatusCode}");

        var content = await response.Content.ReadFromJsonAsync<NpbPriceDto[]>(cancellationToken);

        if (content == null)
            throw new ApplicationException("Failed to deserialize NbpPrices");

        return content;
    }
}