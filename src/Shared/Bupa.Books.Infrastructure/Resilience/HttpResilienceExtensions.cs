using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Bupa.Books.Infrastructure.Resilience;

public static class HttpResilienceExtensions
{
    public static IHttpClientBuilder AddResiliencePolicies(
        this IHttpClientBuilder builder,
        ResilienceOptions options)
    {
        builder.AddResilienceHandler("resilience", pipeline =>
        {
            // Outermost: retry on transient failures (5xx, timeouts, network errors)
            pipeline.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = options.Retry.Count,
                Delay = TimeSpan.FromSeconds(options.Retry.DelaySeconds),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
            });

            // Middle: open circuit after sustained failures to fast-fail downstream calls
            pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = options.CircuitBreaker.FailureThreshold,
                MinimumThroughput = options.CircuitBreaker.MinimumThroughput,
                SamplingDuration = TimeSpan.FromSeconds(options.CircuitBreaker.SamplingDurationSeconds),
                BreakDuration = TimeSpan.FromSeconds(options.CircuitBreaker.DurationOfBreakSeconds),
            });

            // Innermost: per-attempt timeout (applied inside each retry attempt)
            pipeline.AddTimeout(TimeSpan.FromSeconds(options.TimeoutSeconds));
        });

        return builder;
    }
}
