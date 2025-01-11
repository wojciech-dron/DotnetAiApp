using System.ComponentModel;
using DotnetAiApp.Core.Utils;
using Microsoft.SemanticKernel;

namespace DotnetAiApp.Agents.Plugins;

public class FileProviderPlugin
{
    private readonly IFileProvider _fileProvider;

    public FileProviderPlugin(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    [KernelFunction, Description("Saves provided string content as is to the file. Returns output path")]
    public async Task<string> WriteFileContent(string fileName, string content)
    {
        var outputPath = await _fileProvider.WriteTextAsync(fileName, content);

        return $"Content saved to {outputPath}";
    }
}