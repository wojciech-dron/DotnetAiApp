using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NbpApp.NbpApiClient.DelegatingHandlers;

namespace NbpApp.NbpApiClient;

public static class Setup
{
    public static IServiceCollection AddNpbApiClient(this IServiceCollection services)
    {
        services.AddScoped<LoggingDelegatingHandler>();

        services.AddScoped<INbpApiClient, NbpApiClient>();
        services.AddHttpClient<INbpApiClient, NbpApiClient>((serviceProvider, httpClient) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var url = configuration["NbpApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Missing NBP API base URL configuration");

            httpClient.BaseAddress = new Uri(url);
        }).AddHttpMessageHandler<LoggingDelegatingHandler>();

        return services;
    }
}