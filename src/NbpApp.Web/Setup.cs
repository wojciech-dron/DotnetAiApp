using MediatR.Pipeline;
using NbpApp.NbpApiClient;
using NbpApp.Web.Logic.Behaviours;

namespace NbpApp.Web;

public static class Setup
{
    public static IServiceCollection AddNbpAppWebServices(this IServiceCollection services)
    {
        services
            .AddNpbApiClient()
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<INbpAppMarker>();
            });

        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(ExceptionLoggingHandler<,,>));


        return services;
    }
}

interface INbpAppMarker;