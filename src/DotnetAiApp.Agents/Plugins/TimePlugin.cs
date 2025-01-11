using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace DotnetAiApp.Agents.Plugins;

/// <summary>
/// Simple plugin that just returns the time.
/// </summary>
public class TimePlugin
{
    [KernelFunction, Description("Get the current date")]
    public string CurrentDate() => DateTimeOffset.Now.ToString("yyyy-mm-dd");

    [KernelFunction, Description("Get the current time")]
    public DateTimeOffset CurrentTime() => DateTimeOffset.Now;
}
