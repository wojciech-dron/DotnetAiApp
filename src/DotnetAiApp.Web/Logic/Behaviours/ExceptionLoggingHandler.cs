using System.Text.Json;
using DotnetAiApp.Core.Utils;
using Mediator;

namespace DotnetAiApp.Web.Logic.Behaviours;

public class ExceptionLoggingHandler<TRequest, TResponse>
    : MessageExceptionHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : AppResult, new()
{
    private readonly ILogger<ExceptionLoggingHandler<TRequest, TResponse>> _logger;

    public ExceptionLoggingHandler(ILogger<ExceptionLoggingHandler<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    protected override ValueTask<ExceptionHandlingResult<TResponse>> Handle(TRequest message,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred while handling the request.\r\n" +
            "Message: {Message}\r\n"                                                    +
            "Request body: {RequestBody}",
            exception.Message, JsonSerializer.Serialize(message));

        return Handled(null!);
    }
}
