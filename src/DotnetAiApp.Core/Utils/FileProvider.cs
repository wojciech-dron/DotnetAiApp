using Microsoft.Extensions.Configuration;

namespace DotnetAiApp.Core.Utils;

public interface IFileProvider
{
    public Task<string> WriteTextAsync(string path, string content,
        CancellationToken cancellationToken = default);

    public Task<string> AppendTextAsync(string path, string content,
        CancellationToken cancellationToken = default);

    public Task<bool> ExistsAsync(string path,
        CancellationToken cancellationToken = default);
}

public class StaticFileProvider : IFileProvider
{
    private readonly IConfiguration _configuration;

    public StaticFileProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> WriteTextAsync(string path, string content,
        CancellationToken cancellationToken = default)
    {
        var filePath = PrepareFile(path);

        await File.WriteAllTextAsync(filePath, content, cancellationToken);

        return filePath;
    }

    public async Task<string> AppendTextAsync(string path, string content,
        CancellationToken cancellationToken = default)
    {
        var filePath = PrepareFile(path);

        await File.AppendAllTextAsync(filePath, content, cancellationToken);

        return filePath;
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(File.Exists(path));
    }

    private string PrepareFile(string path)
    {
        var uploadRootPath = _configuration["FileProvider:UploadPath"];

        if (uploadRootPath is null)
            throw new ApplicationException("Missing UploadPath for file provider");

        var filePath = Path.Combine(uploadRootPath, path);
        EnsureDirectoryExists(filePath);
        return filePath;
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        var uploadDirectory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(uploadDirectory))
            Directory.CreateDirectory(uploadDirectory);
    }
}