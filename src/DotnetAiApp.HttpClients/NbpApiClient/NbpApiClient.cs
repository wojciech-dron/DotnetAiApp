using System.Net.Http.Json;

namespace DotnetAiApp.NbpApiClient.NbpApiClient;

public interface INbpApiClient
{
    public Task<NpbPriceDto[]> GetGoldPricesAsync(
        IGetGoldPricesRequest request,
        CancellationToken cancellationToken = default);
}

internal class NbpApiClient : INbpApiClient
{
    private readonly HttpClient _client;

    public NbpApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<NpbPriceDto[]> GetGoldPricesAsync(IGetGoldPricesRequest request,
        CancellationToken cancellationToken = default)
    {
        var requestUrl = $"cenyzlota/{request.StartDate:yyyy-MM-dd}/{request.EndDate:yyyy-MM-dd}";

        var response = await _client.GetAsync(requestUrl, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Failed to get gold prices. Status code: {response.StatusCode}");

        var content = await response.Content.ReadFromJsonAsync<NpbPriceDto[]>(cancellationToken);

        if (content == null)
            throw new ApplicationException("Failed to deserialize NbpPrices");

        return content;
    }
}


public interface IGetGoldPricesRequest
{
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }
}