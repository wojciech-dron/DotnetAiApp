using System.Net.Http.Json;
using System.Text;
using NbpApp.NbpApiClient.Contracts;

namespace NbpApp.NbpApiClient;

public interface INbpApiClient
{
    public Task<NpbPrice[]> GetGoldPricesAsync(
        DateOnly startDate,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default);
}

internal class NbpApiClient : INbpApiClient
{
    private readonly HttpClient _client;

    public NbpApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<NpbPrice[]> GetGoldPricesAsync(DateOnly startDate, DateOnly? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var requestUrl = new StringBuilder($"cenyzlota/{startDate:yyyy-MM-dd}");

        if (endDate.HasValue)
            requestUrl.Append($"/{endDate.Value:yyyy-MM-dd}");

        var response = await _client.GetAsync(requestUrl.ToString(), cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Failed to get gold prices. Status code: {response.StatusCode}");

        var content = await response.Content.ReadFromJsonAsync<NpbPrice[]>(cancellationToken);

        if (content == null)
            throw new ApplicationException("Failed to deserialize NbpPrices");

        return content;
    }
}