using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NbpApp.NbpApiClient.DelegatingHandlers;

namespace NbpApp.NbpApiClient;

public static class Setup
{
    public static IServiceCollection AddNpbApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<INbpApiClient>((serviceProvider, httpClient) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var url = configuration["NbpApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Missing NBP API base URL configuration");

            httpClient.BaseAddress = new Uri(url);
        }).AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddScoped<INbpApiClient, NbpApiClient>();

        return services;
    }
}