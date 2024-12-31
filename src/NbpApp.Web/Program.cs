using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using NbpApp.Db;
using NbpApp.Web;
using NbpApp.Web.Components;
#pragma warning disable SKEXP0070

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddNbpAppWebServices(builder.Configuration)
    .AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Services.PrepareDb();

await TestPrompt(app.Services);

app.Run();


async Task TestPrompt(IServiceProvider sp)
{
    using var scope =  sp.CreateScope();
    var serviceProvider = scope.ServiceProvider;

    var chatService = serviceProvider.GetRequiredService<IChatCompletionService>();
    var kernel = serviceProvider.GetRequiredService<Kernel>();
    var settings = new OllamaPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        Temperature = 0.1f
    };

    var prompt = """
                 Get current time and get gold price for last 5 days, then save them to file.

                 """;

    var result = await chatService.GetChatMessageContentAsync(
        prompt,
        executionSettings: settings,
        kernel: kernel);

    Console.WriteLine(result.ToString());
}
