using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NbpApp.NbpApiClient.DelegatingHandlers;
using NbpApp.NbpApiClient.Validators;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace NbpApp.NbpApiClient;

public static class Setup
{
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
        HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy =
        HttpPolicyExtensions.HandleTransientHttpError()
            .CircuitBreakerAsync(3, TimeSpan.FromMinutes(1));

    private static readonly AsyncPolicy<HttpResponseMessage> ResilientPolicy =
        CircuitBreakerPolicy.WrapAsync(RetryPolicy);

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
            })
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .AddPolicyHandler(ResilientPolicy);

        services.AddScoped<GoldPricesRequestValidator>();
        services.AddScoped<IValidator<IGetGoldPricesRequest>>(sp =>
            sp.GetRequiredService<GoldPricesRequestValidator>());

        return services;
    }
}