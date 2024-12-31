﻿using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using NbpApp.Ai.Plugins;
using NbpApp.Ai.Settings;
using OllamaSharp;
#pragma warning disable SKEXP0001

namespace NbpApp.Ai;

public static class Setup
{
    public static IServiceCollection AddAiModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiSettings>(configuration.GetSection(AiSettings.SectionName));

        services.AddScoped<NbpPlugin>();
        services.AddScoped<FileProviderPlugin>();

        services.AddScoped<IChatCompletionService>(sp =>
        {
            var loggerFactory = sp.GetService<ILoggerFactory>();
            var settings = sp.GetRequiredService<IOptions<AiSettings>>().Value;

            var builder = ((IChatClient)new OllamaApiClient(settings.OllamaEndpoint, settings.ModelId))
                .AsBuilder()
                .UseFunctionInvocation(loggerFactory);

            if (loggerFactory is not null)
            {
                builder.UseLogging(loggerFactory);
            }

            return builder.Build(sp).AsChatCompletionService(sp);
        });

        services.AddScoped<Kernel>(sp =>
        {
            var pluginCollection = new KernelPluginCollection();

            pluginCollection.AddFromType<MyTimePlugin>();
            pluginCollection.AddFromObject(sp.GetRequiredService<NbpPlugin>());
            pluginCollection.AddFromObject(sp.GetRequiredService<FileProviderPlugin>());

            return new Kernel(sp, pluginCollection);
        });

        return services;
    }
}