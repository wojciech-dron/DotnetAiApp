# NBP APP

This is demo for fetching gold prices from [NBP API](https://api.nbp.pl/en.html)
with db and file saving.

Demo provides basic ai features.

## Getting started

To run application, execute the following command in the project root folder:
```bash
    dotnet run -p .\src\NbpApp.Web\
```

## Pre-requisites


To enable ai features you need tohave docker installed 
and run ollama locally with llama3.2 model.

Run ollama on docker with port 11434:

```bash
   docker run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
```

Run llama3.2 on ollama:

```bash
  docker exec -it ollama ollama run llama3.2
```

If model responses are very small consider using smaller version of llama3.2:1b.

```bash
  docker exec -it ollama ollama run llama3.2:1b
```

It requires change in appsettings.json file:

```
"AiSettings.ModelId": "llama3.2:1b"
```