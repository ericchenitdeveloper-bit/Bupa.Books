using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Bupa.Books.Common.Extensions;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddCommonApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddMvc();
        services.AddHealthChecks();
        return services;
    }
}
