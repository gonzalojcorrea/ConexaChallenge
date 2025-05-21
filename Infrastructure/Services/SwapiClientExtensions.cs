using Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Infrastructure.Services;

/// <summary>
/// Extension methods for adding SwapiClient with resilience policies.
/// </summary>
public static class SwapiClientExtensions
{
    /// <summary>
    /// Registers SwapiClient with HttpClient and Polly resilience policies (retry, circuit breaker, timeout) including logging.
    /// </summary>
    public static IServiceCollection AddSwapiClient(this IServiceCollection services)
    {
        services.AddHttpClient<ISwapiClient, SwapiClient>(client =>
        {
            client.BaseAddress = new Uri("https://www.swapi.tech/api/");
        })
        .AddPolicyHandler((sp, request) => GetRetryPolicy(sp.GetRequiredService<ILogger<SwapiClient>>()))
        .AddPolicyHandler((sp, request) => GetCircuitBreakerPolicy(sp.GetRequiredService<ILogger<SwapiClient>>()))
        .AddPolicyHandler(GetTimeoutPolicy());

        return services;
    }

    /// <summary>
    /// Creates a retry policy for transient HTTP errors with exponential backoff and logging.
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                // Linear backoff: wait 5 seconds per retry attemp
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(5 * attempt),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    logger.LogWarning(
                        "Retry attempt {RetryAttempt} after {Delay}s due to: {ExceptionMessage}",
                        retryAttempt,
                        timespan.TotalSeconds,
                        outcome.Exception?.Message
                    );
                }
            );

    /// <summary>
    /// Creates a circuit breaker policy for transient HTTP errors with logging.
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(60),
                onBreak: (outcome, timespan) =>
                {
                    logger.LogError(
                        "Circuit broken! External service unavailable for {BreakDuration}s. Error: {ExceptionMessage}",
                        timespan.TotalSeconds,
                        outcome.Exception?.Message
                    );
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit reset. External service is available again.");
                }
            );

    /// <summary>
    /// Creates a timeout policy for HTTP requests with a pessimistic timeout strategy.
    /// </summary>
    /// <returns></returns>
    private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy() =>
        Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(20),
            TimeoutStrategy.Pessimistic
        );
}
