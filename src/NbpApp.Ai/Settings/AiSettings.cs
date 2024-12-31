namespace NbpApp.Ai.Settings;

public class AiSettings
{
    public const string SectionName = "AiSettings";

    public required string OllamaEndpoint { get; set; }
    public required string ModelId { get; set; }
}