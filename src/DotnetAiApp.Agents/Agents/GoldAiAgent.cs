using DotnetAiApp.Agents.Plugins;
using DotnetAiApp.Core.Utils;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace DotnetAiApp.Agents.Agents;

public class GoldAiAgent
{
    public static readonly string SystemPrompt =
        $"""
        You are a helpful assistant that helps getting and managing data about gold prices in polish zloty.
        Show data in simple table.
        Today is {DateTime.Now:u}.
        Do not write any code or scripts, if neccessary call functions from plugins:
        {nameof(NbpApiPlugin)}, {nameof(FileProviderPlugin)} and {nameof(TimePlugin)}.
        If you don't know the answer, just say that you don't know. Don't try to make up an answer.
        Do not use any other plugins except those mentioned above.
        """;

    public record Request(ChatHistory History) : IRequest<Result>;

    internal class Handler : IRequestHandler<Request, Result>
    {
        private readonly IChatCompletionService _chatService;
        private readonly Kernel _kernel;

        private static readonly OllamaPromptExecutionSettings _chatSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = 0.0f,
        };

        public Handler(IChatCompletionService chatService,
            IServiceProvider serviceProvider)
        {
            _chatService = chatService;

            _kernel = new Kernel(serviceProvider);
            _kernel.Plugins.AddFromType<TimePlugin>();
            _kernel.Plugins.AddFromObject(serviceProvider.GetRequiredService<NbpApiPlugin>());
            _kernel.Plugins.AddFromObject(serviceProvider.GetRequiredService<FileProviderPlugin>());
        }

        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var history = request.History;

            try
            {
                var result = await _chatService.GetChatMessageContentAsync(
                    history,
                    executionSettings: _chatSettings,
                    kernel: _kernel,
                    cancellationToken: cancellationToken);

                history.AddMessage(result.Role, result.Content ?? string.Empty);
            }
            catch (HttpRequestException)
            {
                return new Result(history) { ErrorMessage = "Unable to connect to the Ollama API." };
            }

            return history;
        }
    }

    public class Result(ChatHistory history) : AppResult
    {
        public ChatHistory History { get; } = history;

        public static implicit operator Result(ChatHistory history) => new(history);
    }
}