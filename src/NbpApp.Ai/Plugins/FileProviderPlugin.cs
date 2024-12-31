using System.ComponentModel;
using Microsoft.SemanticKernel;
using NbpApp.Utils.FileProvider;

namespace NbpApp.Ai.Plugins;

public class FileProviderPlugin
{
    private readonly IFileProvider _fileProvider;

    public FileProviderPlugin(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    [KernelFunction, Description("Saves provided string content to file")]
    [return: Description("Result message with output path")]
    public async Task<string> WriteFile(string fileName, string content)
    {
        var outputPath = await _fileProvider.WriteTextAsync(fileName, content);

        return $"Content saved to {outputPath}";
    }
}