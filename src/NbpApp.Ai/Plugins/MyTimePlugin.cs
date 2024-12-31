﻿using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace NbpApp.Ai.Plugins;

/// <summary>
/// Simple plugin that just returns the time.
/// </summary>
public class MyTimePlugin
{
    [KernelFunction, Description("Get the current date")]
    public string CurrentDate() => DateTimeOffset.Now.ToString("yyyy-mm-dd");

    [KernelFunction, Description("Get the current time")]
    public DateTimeOffset CurrentTime() => DateTimeOffset.Now;
}
