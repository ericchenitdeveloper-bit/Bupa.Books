using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Bupa.Books.Common.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BrokenCircuitException ex)
        {
            logger.LogWarning(ex, "Circuit breaker open for {Path}", context.Request.Path);
            await WriteProblem(context, HttpStatusCode.BadGateway,
                "Bad Gateway", "The upstream service is temporarily unavailable.");
        }
        catch (TimeoutRejectedException ex)
        {
            logger.LogError(ex, "Polly timeout exceeded for {Path}", context.Request.Path);
            await WriteProblem(context, HttpStatusCode.GatewayTimeout,
                "Gateway Timeout", "The upstream service did not respond in time.");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "External API request failed for {Path}", context.Request.Path);
            await WriteProblem(context, HttpStatusCode.BadGateway,
                "Bad Gateway", "Failed to retrieve data from the upstream service.");
        }
        catch (TaskCanceledException ex) when (!context.RequestAborted.IsCancellationRequested)
        {
            logger.LogError(ex, "External API timed out for {Path}", context.Request.Path);
            await WriteProblem(context, HttpStatusCode.GatewayTimeout,
                "Gateway Timeout", "The upstream service did not respond in time.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);
            await WriteProblem(context, HttpStatusCode.InternalServerError,
                "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private static Task WriteProblem(HttpContext context, HttpStatusCode status, string title, string detail)
    {
        var code = (int)status;
        context.Response.StatusCode = code;
        context.Response.ContentType = "application/problem+json";
        return context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = code,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.io/{code}"
        });
    }
}
