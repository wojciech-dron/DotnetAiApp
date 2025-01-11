using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetAiApp.Db;

public static class Setup
{
    public static IServiceCollection AddDotnetAiAppDb(this IServiceCollection services)
    {
        services.AddDbContext<DotentAiAppContext>((serviceProvider, options) =>
        {
            var connectionString = serviceProvider
                .GetRequiredService<IConfiguration>()
                .GetConnectionString("NbpDbConnection");

            options.UseSqlite(connectionString, o =>
                o.MigrationsAssembly(typeof(Setup).Assembly));
        });

        return services;
    }

    public static void PrepareDb(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DotentAiAppContext>();
        dbContext.Database.EnsureCreated();
    }
}