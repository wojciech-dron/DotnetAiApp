using Microsoft.Extensions.DependencyInjection;
using NbpApp.Utils.FileProvider;

namespace NbpApp.Utils;

public static class Setup
{
    public static IServiceCollection AddStaticFileProvider(this IServiceCollection services)
    {
        return services.AddScoped<IFileProvider, StaticFileProvider>();
    }
}