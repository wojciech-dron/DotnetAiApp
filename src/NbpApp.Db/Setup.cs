using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NbpApp.Db.Repos;

namespace NbpApp.Db;

public static class Setup
{
    public static IServiceCollection AddNbpAppDb(this IServiceCollection services)
    {
        services.AddScoped<IGoldPriceRepository, GoldPriceRepository>();

        services.AddDbContext<NbpAppContext>((serviceProvider, options) =>
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
        var dbContext = scope.ServiceProvider.GetRequiredService<NbpAppContext>();
        dbContext.Database.EnsureCreated();
    }
}