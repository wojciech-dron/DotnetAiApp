using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using NbpApp.Ai;
using NbpApp.Db;
using NbpApp.NbpApiClient;
using NbpApp.Utils;
using NbpApp.Web.Logic.Behaviours;

namespace NbpApp.Web;

public static class Setup
{
    public static IServiceCollection AddNbpAppWebServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(Setup).Assembly);
        services.AddMediatrForNbpApp();

        services.AddUtils();
        services.AddNbpAppDb();
        services.AddNpbApiClient();
        services.AddAiModule(configuration);

        return services;
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

