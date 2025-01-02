using FluentValidation;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Localization;
using Morris.Blazor.Validation;
using NbpApp.Ai;
using NbpApp.Db;
using NbpApp.NbpApiClient;
using NbpApp.Utils;
using NbpApp.Web.Logic.Behaviours;

namespace NbpApp.Web;

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
        services.AddMediatrForNbpApp();

        services.AddUtils();
        services.AddNbpAppDb();
        services.AddNpbApiClient();
        services.AddAiModule(configuration);

        return services;
    }

    public static void SetAppCulture(this WebApplication webApplication)
    {
        var cultureOptions = new RequestLocalizationOptions()
            .AddSupportedCultures("en-EN")
            .AddSupportedUICultures("en-EN");

        var requestCulture = new RequestCulture("en-EN");
        requestCulture.Culture.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
        cultureOptions.DefaultRequestCulture = requestCulture;

        webApplication.UseRequestLocalization(cultureOptions);
    }

    private static IServiceCollection AddMediatrForNbpApp(this IServiceCollection services)
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

