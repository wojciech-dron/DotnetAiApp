using Microsoft.Extensions.Logging;

namespace NbpApp.NbpApiClient.DelegatingHandlers;

public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingDelegatingHandler> _logger;

    public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestUrl = request.RequestUri?.AbsoluteUri;
        var method = request.Method;
        var requestBody = await GetRequestBody(request, cancellationToken);

        try
        {
            _logger.LogTrace("Sending request to {RequestUrl}", requestUrl);

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
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode) // 2XX
            {
                _logger.LogTrace("Received {Status} response: {Method} {RequestUrl}, response: {ResponseBody}",
                    response.StatusCode, method, requestUrl, responseBody);
            }
            else
            {
                _logger.LogWarning("Received {Status} response: {Method} {RequestUrl}, request body: {RequestBody}, " +
                                   "response body: {ResponseBody}",
                    response.StatusCode, method, requestUrl, requestBody, responseBody);
            }
        }
    }

    private static async Task<string> GetRequestBody(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Content is null)
            return null;

        return await request.Content.ReadAsStringAsync(cancellationToken);
    }
}