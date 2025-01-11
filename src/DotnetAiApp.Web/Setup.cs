using FluentValidation;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Localization;
using Morris.Blazor.Validation;
using DotnetAiApp.Agents;
using DotnetAiApp.Core;
using DotnetAiApp.Web.Logic.Behaviours;
using DotnetAiApp.Db;
using DotnetAiApp.NbpApiClient;

namespace DotnetAiApp.Web;

public static class Setup
{
    public static IServiceCollection AddAppServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddBlazorBootstrap();

        services.AddFormValidation(config =>
            config.AddFluentValidation(typeof(Setup).Assembly)
        );

        services.AddValidatorsFromAssembly(typeof(Setup).Assembly);
        services.AddMediatrForDotnetAiApp();

        services.AddUtils();
        services.AddDotnetAiAppDb();
        services.AddNpbApiClient();
        services.AddAiModule(configuration);

        return services;
    }

    public static void SetAppCulture(this WebApplication webApplication)
    {
        var cultureOptions = new RequestLocalizationOptions()
            .AddSupportedCultures("en-EN", "pl-PL")
            .AddSupportedUICultures("en-EN");

        var requestCulture = new RequestCulture("pl-PL");
        requestCulture.Culture.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
        cultureOptions.DefaultRequestCulture = requestCulture;

        webApplication.UseRequestLocalization(cultureOptions);
    }

    private static IServiceCollection AddMediatrForDotnetAiApp(this IServiceCollection services)
    {
        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(ExceptionLoggingHandler<,,>));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Setup).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }



}

