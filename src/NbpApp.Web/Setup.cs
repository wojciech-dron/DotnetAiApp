using MediatR.Pipeline;
using NbpApp.Db;
using NbpApp.NbpApiClient;
using NbpApp.Web.Logic.Behaviours;

namespace NbpApp.Web;

public static class Setup
{
    public static IServiceCollection AddNbpAppWebServices(this IServiceCollection services)
    {
        services
            .AddMediatrForNbpApp()
            .AddNbpAppDb()
            .AddNpbApiClient();

        return services;
    }

    private static IServiceCollection AddMediatrForNbpApp(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Setup).Assembly);
        });

        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(ExceptionLoggingHandler<,,>));

        return services;
    }
}

