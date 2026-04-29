namespace Bupa.Books.Infrastructure.Resilience;

public class ResilienceOptions
{
    public int TimeoutSeconds { get; set; } = 30;
    public RetryOptions Retry { get; set; } = new();
    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();
}

public class RetryOptions
{
    public int Count { get; set; } = 3;
    public double DelaySeconds { get; set; } = 2;
}

public class CircuitBreakerOptions
{
    public double FailureThreshold { get; set; } = 0.5;
    public int MinimumThroughput { get; set; } = 10;
    public double SamplingDurationSeconds { get; set; } = 30;
    public double DurationOfBreakSeconds { get; set; } = 30;
}
