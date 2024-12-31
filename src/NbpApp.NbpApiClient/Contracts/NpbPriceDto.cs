using System.Text.Json.Serialization;

namespace NbpApp.NbpApiClient.Contracts;

public class NpbPriceDto
{
    [JsonPropertyName("data")]
    public DateOnly Date { get; init; }

    [JsonPropertyName("cena")]
    public decimal Price { get; init; }

}