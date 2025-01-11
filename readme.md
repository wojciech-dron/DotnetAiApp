# Dotnet AI App

This is demo for basic ai features.

Demo provides fetching gold prices from [NBP API](https://api.nbp.pl/en.html)
with db and file saving.


## Getting started

To run application, execute the following command in the project root folder:
```bash
    dotnet run -p .\src\DotnetAiApp.Web\
```

## Pre-requisites

To enable ai features you need tohave docker installed 
and run ollama locally with llama3.1:8b model.

#### Run ollama on docker with port 11434:

```bash
   docker run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
```

#### Run llama3.2 on ollama:

```bash
  docker exec -it ollama ollama run llama3.1:8b
```

### Model support 
This app supports all models with [tools](https://ollama.com/search?c=tools).
It requires pulling model with ollama and change in appsettings.json file.
If you don't have GPU with at least 8GB VRAM consider using smaller models like llama3.2:1b

Example:

```bash
  docker exec -it ollama ollama run llama3.2:1b
```

appsettings.json section:

```
"AiSettings.ModelId": "llama3.2:1b"
```