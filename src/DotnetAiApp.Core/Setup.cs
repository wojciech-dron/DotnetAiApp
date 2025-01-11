using DotnetAiApp.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using TimeProvider = DotnetAiApp.Core.Utils.TimeProvider;

namespace DotnetAiApp.Core;

public static class Setup
{
    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        services.AddTransient<ITimeProvider, TimeProvider>();
        services.AddScoped<IFileProvider, StaticFileProvider>();

        return services;
    }
}