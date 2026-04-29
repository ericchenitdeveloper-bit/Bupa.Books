using Bupa.Books.Common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Bupa.Books.Common.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseCommonExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    public static WebApplication UseCorrelationId(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }

    public static WebApplication MapCommonEndpoints(this WebApplication app)
    {
        app.UseRateLimiter();
        app.MapControllers().RequireRateLimiting("api");
        app.MapHealthChecks("/health");
        return app;
    }
}
