# Dotnet AI App

This is demo for basic ai features.
Demo provides fetching gold prices from [NBP API](https://api.nbp.pl/en.html)
with db and file saving.


## Pre-requisites

Ollama is required to enable ai features. Alternatively, you can use OpenAI-compatible services.

### Using Ollama

Install ollama from [official website](https://ollama.com/download), then:.

```bash
    ollama run llama3.1:8b
```

### Using Docker with Ollama
You can also run ollama with docker

```bash
   docker run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
   docker exec -it ollama ollama run llama3.1:8b
```

### Using OpenAI or OpenAI-compatible services
To use OpenAI or other OpenAI-compatible services, you need to configure the AiSettings in appsettings.json with your API key and endpoint.

## Launch app

To run application, execute the following command in the project root folder:
```bash
    dotnet run -p .\src\DotnetAiApp.Web\
```


### Model support
This app supports all models with tools.
You can find models with tools support in: https://ollama.com/search?c=tools.
It requires pulling model with ollama and change in appsettings.json file.
If you don't have GPU with at least 8GB VRAM consider using smaller models like llama3.2:1b

Example for Ollama:

```bash
  ollama run llama3.2:1b
```

Example for OpenAI (uncomment and modify the AiSettings section in appsettings.json):

```json
  "AiSettings": {
    "Provider": "OpenAI",
    "DefaultModelId": "gpt-4o", // or another OpenAI model
    "ApiKey": "your-openai-api-key-here",
    "OpenAIEndpoint": "https://api.openai.com/v1" // Optional, defaults to OpenAI's endpoint
  }
```

