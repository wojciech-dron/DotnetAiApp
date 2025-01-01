using System.Text.Json;
using MediatR;
using MediatR.Pipeline;
using NbpApp.Utils.Utils;

namespace NbpApp.Web.Logic.Behaviours;

public class ExceptionLoggingHandler<TRequest, TResponse, TException>
    : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TResponse : AppResult, new()
    where TException : Exception
{
    private readonly ILogger<ExceptionLoggingHandler<TRequest, TResponse, TException>> _logger;

    public ExceptionLoggingHandler(ILogger<ExceptionLoggingHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred while handling the request.\r\n" +
                                    "Message: {Message}\r\n" +
                                    "Request body: {RequestBody}",
            exception.Message, JsonSerializer.Serialize(request));

        state.SetHandled(new TResponse { ErrorMessage = exception.Message });

        return Task.CompletedTask;
    }
}
