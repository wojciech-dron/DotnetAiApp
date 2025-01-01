using Microsoft.Extensions.DependencyInjection;
using NbpApp.Utils.Utils;
using TimeProvider = NbpApp.Utils.Utils.TimeProvider;

namespace NbpApp.Utils;

public static class Setup
{
    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        services.AddScoped<IFileProvider, StaticFileProvider>();
        services.AddScoped<ITimeProvider, TimeProvider>();

        return services;
    }
}