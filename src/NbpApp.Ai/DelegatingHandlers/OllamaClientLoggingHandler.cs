using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace NbpApp.Ai.DelegatingHandlers;


public class OllamaClientLoggingHandler : DelegatingHandler
{
    private readonly ILogger<OllamaClientLoggingHandler> _logger;
    private readonly ILogger<OllamaChat> _chatLogger;

    public OllamaClientLoggingHandler(ILogger<OllamaClientLoggingHandler> logger,
        // ReSharper disable once ContextualLoggerProblem
        ILogger<OllamaChat> chatLogger)
    {
        _logger = logger;
        _chatLogger = chatLogger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestUrl = request.RequestUri?.AbsoluteUri;
        var requestBody = await GetRequestBody(request, cancellationToken);
        var timeAndType = $",{{\"Request\": \"{DateTimeOffset.Now}\"}},";

        try
        {
            _chatLogger.LogTrace("{TimeAndType}\n{Request}", timeAndType, requestBody);

            var response = await base.SendAsync(request, cancellationToken);

            await LogResponse(response);

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error response from: {RequestUrl}, body: {Body}, message: {Message}",
                requestUrl, requestBody, e.Message);

            throw;
        }

        async Task LogResponse(HttpResponseMessage response)
        {
            var responseBody = await GetResponseBody(cancellationToken, response);
            var timeAndType = $",{{\"Response\": \"{DateTimeOffset.Now}\"}},";

            _chatLogger.LogTrace("{TimeAndType}\n{Response}", timeAndType, responseBody);
        }
    }

    private async Task<string> GetResponseBody(CancellationToken cancellationToken, HttpResponseMessage response)
    {
        if (_chatLogger.IsEnabled(LogLevel.Trace))
            return await response.Content.ReadAsStringAsync(cancellationToken);

        return "";
    }

    private async Task<string?> GetRequestBody(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Content is null)
            return null;

        if (!_chatLogger.IsEnabled(LogLevel.Trace))
            return null;

        return await request.Content.ReadAsStringAsync(cancellationToken);
    }
}

public class OllamaChat;