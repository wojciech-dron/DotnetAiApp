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
        services.AddMediatrForNbpApp();
        services.AddStaticFileProvider();
        services.AddNbpAppDb();
        services.AddNpbApiClient();
        services.AddAiModule(configuration);

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

