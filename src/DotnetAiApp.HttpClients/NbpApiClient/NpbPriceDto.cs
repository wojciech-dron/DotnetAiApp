using System.Text.Json.Serialization;

namespace DotnetAiApp.NbpApiClient.NbpApiClient;

public class NpbPriceDto
{
    [JsonPropertyName("data")]
    public DateTime Date { get; init; }

    [JsonPropertyName("cena")]
    public decimal Price { get; init; }

}