# AI Providers Configuration

This project supports multiple AI providers through the `AiSettings` configuration.

## Supported Providers

### 1. Ollama
- Uses local LLM models via Ollama
- Configuration required: `OllamaEndpoint`, `DefaultModelId`
- Example:
```json
{
  "Provider": "Ollama",
  "OllamaEndpoint": "http://localhost:11434",
  "DefaultModelId": "llama3.1:8b"
}
```

### 2. OpenAI
- Uses OpenAI-compatible API services (OpenAI, Azure OpenAI, etc.)
- Configuration required: `ApiKey`, `DefaultModelId`
- Optional: `OpenAIEndpoint` (defaults to OpenAI's endpoint)
- Example:
```json
{
  "Provider": "OpenAI",
  "DefaultModelId": "gpt-4o",
  "ApiKey": "your-api-key-here"
}
```

## Configuration

The AI settings are configured in `appsettings.json` or environment-specific files like `appsettings.Development.json`.

For OpenAI, you can also create a separate configuration file like `appsettings.OpenAI.Development.json` to easily switch between providers.