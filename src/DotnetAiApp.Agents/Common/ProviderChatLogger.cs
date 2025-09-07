using System.Text.Json;
using DotnetAiApp.Agents.Settings;
using DotnetAiApp.Core.Utils;
using Microsoft.Extensions.Options;

namespace DotnetAiApp.Agents.Common;

public class ProviderChatLogger : DelegatingHandler
{
    private readonly string _outputFileName = $"ollama/{DateTime.Now:dd-MM-yy}.json";

    private readonly IFileProvider _fileProvider;
    private readonly AiSettings _settings;

    public ProviderChatLogger(IFileProvider fileProvider, IOptions<AiSettings> settings)
    {
        _fileProvider = fileProvider;
        _settings = settings.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!_settings.LogChat)
            await base.SendAsync(request, cancellationToken);

        await PrepareFile(cancellationToken);
        await LogRequest(request, cancellationToken);

        var response = await base.SendAsync(request, cancellationToken);

        await LogResponse(response, cancellationToken);

        return response;
    }

    private async Task PrepareFile(CancellationToken cancellationToken)
    {
        if (!await _fileProvider.ExistsAsync(_outputFileName, cancellationToken))
            await _fileProvider.WriteTextAsync(_outputFileName, "[\n", cancellationToken);
    }

    private async Task LogRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content is null)
            return;

        var body = await request.Content.ReadAsStringAsync(cancellationToken);
        body = FormatJsonString(body);
        var timeAndType = $"{{ \"Request\": \"{DateTimeOffset.Now:timeFormat}\" }}";

        await _fileProvider.AppendTextAsync(_outputFileName, $"{timeAndType},\n{body},", cancellationToken);
    }

    private async Task LogResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        body = FormatJsonString(body);
        var timeAndType = $"{{ \"Response\": \"{DateTimeOffset.Now:dd-MM-yy HH:mm z}\" }}";

        await _fileProvider.AppendTextAsync(_outputFileName, $"\n{timeAndType},\n{body},\n", cancellationToken);
    }

    private static string FormatJsonString(string input)
    {
        try
        {
            using var doc = JsonDocument.Parse(input);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException)
        {
            return input;
        }
    }
}
