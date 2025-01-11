using MediatR;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using NbpApp.Ai.Agents;
using NbpApp.Ai.Common;
using NbpApp.Ai.Plugins;
using NbpApp.Ai.Settings;
using NbpApp.NbpApiClient.DelegatingHandlers;
using OllamaSharp;
#pragma warning disable SKEXP0001

namespace NbpApp.Ai;

public static class Setup
{
    public static IServiceCollection AddAiModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AiSettings>(configuration.GetSection(AiSettings.SectionName));

        services.AddScoped<NbpApiPlugin>();
        services.AddScoped<FileProviderPlugin>();
        services.AddScoped<OllamaChatLogger>();

        services.AddScoped<IChatCompletionService>(sp =>
        {
            var loggerFactory = sp.GetService<ILoggerFactory>();
            var settings = sp.GetRequiredService<IOptions<AiSettings>>().Value;
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
        });

        services.AddHttpClient<OllamaApiClient>((sp, c) =>
        {
            var settings = sp.GetRequiredService<IOptions<AiSettings>>().Value;
            c.BaseAddress = new Uri(settings.OllamaEndpoint);
        }).AddHttpMessageHandler<OllamaChatLogger>();

        services.AddGoldAgent();

        return services;
    }

    private static void AddGoldAgent(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<GoldAiAgent.Request, GoldAiAgent.Result>, GoldAiAgent.Handler>();
    }
}