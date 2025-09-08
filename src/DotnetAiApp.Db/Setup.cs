using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetAiApp.Db;

public static class Setup
{
    public static IServiceCollection AddDotnetAiAppDb(this IServiceCollection services)
    {
        services.AddEFSecondLevelCache(options =>
            options.UseMemoryCacheProvider()
                .UseCacheKeyPrefix("EF_")
                .UseDbCallsIfCachingProviderIsDown(TimeSpan.FromMinutes(1))
        );

        services.AddDbContext<DotentAiAppContext>((serviceProvider, options) =>
        {
            var connectionString = serviceProvider
                .GetRequiredService<IConfiguration>()
                .GetConnectionString("NbpDbConnection");

            options.UseSqlite(connectionString, o =>
                o.MigrationsAssembly(typeof(Setup).Assembly));

            var cacheInterceptor = serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>();
            options.AddInterceptors(cacheInterceptor);
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