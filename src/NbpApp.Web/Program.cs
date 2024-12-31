using NbpApp.Db;
using NbpApp.Web;
using NbpApp.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddNbpAppWebServices()
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

app.Run();