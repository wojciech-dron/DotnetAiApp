using DotnetAiApp.Agents.Agents;
using DotnetAiApp.Agents.Common;
using DotnetAiApp.Agents.Plugins;
using DotnetAiApp.Agents.Settings;
using Mediator;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;

#pragma warning disable SKEXP0001

namespace DotnetAiApp.Agents;

public static class Setup
{
    public static IServiceCollection AddAiModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AiSettings>(configuration.GetSection(AiSettings.SectionName));

        services.AddScoped<NbpApiPlugin>();
        services.AddScoped<FileProviderPlugin>();
        services.AddScoped<ProviderChatLogger>();

        services.AddScoped<IChatCompletionService>(sp =>
        {
            var loggerFactory = sp.GetService<ILoggerFactory>();
            var settings = sp.GetRequiredService<IOptions<AiSettings>>().Value;

            return settings.Provider.ToLower() switch
            {
                "ollama" => CreateOllamaChatCompletionService(sp, loggerFactory, settings),
                "openai" => CreateOpenAiChatCompletionService(sp, settings),
                _        => throw new ArgumentException($"Unsupported AI provider: {settings.Provider}")
            };
        });

        // Register OpenAI-specific services
        services.AddHttpClient("OpenAi", (sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<AiSettings>>().Value;
            client.BaseAddress = new Uri(settings.OpenAIEndpoint ?? "https://api.openai.com/v1");
        }).AddHttpMessageHandler<ProviderChatLogger>();

        // Register Ollama-specific services
        services.AddHttpClient<OllamaApiClient>((sp, c) =>
        {
            var settings = sp.GetRequiredService<IOptions<AiSettings>>().Value;
            c.BaseAddress = new Uri(settings.OllamaEndpoint);
        }).AddHttpMessageHandler<ProviderChatLogger>();

        services.AddGoldAgent();

        return services;
    }

    private static IChatCompletionService CreateOllamaChatCompletionService(IServiceProvider sp, ILoggerFactory? loggerFactory, AiSettings settings)
    {
        var ollamaApiClient = sp.GetRequiredService<OllamaApiClient>();
        ollamaApiClient.SelectedModel = settings.DefaultModelId;
        var builder = ((IChatClient)ollamaApiClient)
            .AsBuilder()
            .UseFunctionInvocation(loggerFactory);

        if (loggerFactory is not null)
        {
            builder.UseLogging(loggerFactory);
        }

        return builder.Build(sp).AsChatCompletionService(sp);
    }

    private static IChatCompletionService CreateOpenAiChatCompletionService(IServiceProvider serviceProvider,
        AiSettings settings)
    {
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("OpenAi");

        var builder = Kernel
            .CreateBuilder()
            .AddOpenAIChatCompletion(
                settings.DefaultModelId,
                settings.ApiKey ?? "",
                httpClient: httpClient);

        return builder.Build().Services.GetRequiredService<IChatCompletionService>();
    }

    private static void AddGoldAgent(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<GoldAiAgent.Request, GoldAiAgent.Result>, GoldAiAgent.Handler>();
    }
}