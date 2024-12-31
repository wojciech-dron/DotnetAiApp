namespace NbpApp.Web.FileProvider;

public interface IFileProvider
{
    public Task WriteTextAsync(string fileName, string content, CancellationToken cancellationToken = default);
}

public class StaticFileProvider : IFileProvider
{
    private readonly IConfiguration _configuration;

    public StaticFileProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task WriteTextAsync(string fileName, string content, CancellationToken cancellationToken = default)
    {
        var uploadPath = _configuration["FileProvider:UploadPath"];

        if (uploadPath is null)
            throw new ApplicationException("Missing UploadPath for file provider");

        Directory.CreateDirectory(Path.Combine(uploadPath));

        var filePath = Path.Combine(uploadPath, fileName);
        await File.WriteAllTextAsync(filePath, content, cancellationToken);
    }
}