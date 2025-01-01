using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace NbpApp.Web.Logic;

public class GoldAiAgent
{
    public static readonly string SystemPrompt =
        $$$"""
        
        You are a helpful assistant that helps people find information about the world.
        You will be given a question and you must answer it.
        If necessary you can use plugin functions.
        Current time is {{{DateTimeOffset.Now}}}.
        
        """;

    public record Request(ChatHistory History) : IRequest<Result>;

    internal class Handler : IRequestHandler<Request, Result>
    {
        private readonly IChatCompletionService _chatService;
        private readonly Kernel _kernel;

        private static readonly OllamaPromptExecutionSettings _chatSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        public Handler(IChatCompletionService chatService, Kernel kernel)
        {
            _chatService = chatService;
            _kernel = kernel;
        }

        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var history = request.History;

            var result = await _chatService.GetChatMessageContentAsync(
                history,
                executionSettings: _chatSettings,
                kernel: _kernel,
                cancellationToken: cancellationToken);

            history.AddMessage(result.Role, result.Content ?? string.Empty);

            return history;
        }
    }

    public record Result(ChatHistory History)
    {
        public static implicit operator Result(ChatHistory history) => new(history);
    }
}