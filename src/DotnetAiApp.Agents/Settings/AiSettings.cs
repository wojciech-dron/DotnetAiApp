namespace DotnetAiApp.Agents.Settings;

public class AiSettings
{
    public const string SectionName = "AiSettings";

    public required string Provider { get; set; } // "Ollama" or "OpenAI"
    public required string OllamaEndpoint { get; set; }
    public required string DefaultModelId { get; set; }
    public bool LogChat { get; set; }
    public string? ApiKey { get; set; }
    public string? OpenAIEndpoint { get; set; } // For OpenAI-like providers
}