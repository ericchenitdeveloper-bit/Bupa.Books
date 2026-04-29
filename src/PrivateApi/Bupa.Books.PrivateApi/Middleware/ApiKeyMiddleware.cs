namespace Bupa.Books.PrivateApi.Middleware;

public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
{
    private const string ApiKeyHeader = "X-Api-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var providedKey))
        {
            logger.LogWarning("Request missing API key from {IP}", context.Connection.RemoteIpAddress);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API key required." });
            return;
        }

        var expectedKey = configuration["PrivateApi:ApiKey"];
        if (providedKey != expectedKey)
        {
            logger.LogWarning("Invalid API key from {IP}", context.Connection.RemoteIpAddress);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid API key." });
            return;
        }

        await next(context);
    }
}
