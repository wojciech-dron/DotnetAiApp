namespace DotnetAiApp.Agents.Settings;

public class AiSettings
{
    public const string SectionName = "AiSettings";

    public required string OllamaEndpoint { get; set; }
    public required string DefaultModelId { get; set; }
    public bool LogChat { get; set; }
}