using System.Reflection;
using System.Threading.RateLimiting;
using Bupa.Books.Application.Books.Interfaces;
using Bupa.Books.Application.Books.Queries;
using Bupa.Books.Common.Extensions;
using Bupa.Books.Infrastructure.ExternalApi;
using Bupa.Books.Infrastructure.Resilience;
using Bupa.Books.PrivateApi.Middleware;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCommonApiServices();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Bupa Books Private API", Version = "v1" });

    foreach (var xmlFile in new[]
             {
                 $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
                 $"{typeof(CategorizedBooksResponse).Assembly.GetName().Name}.xml"
             })
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<GetCategorizedBooksQuery>());

var booksApiResilience = builder.Configuration
    .GetSection("BooksApi:Resilience")
    .Get<ResilienceOptions>() ?? new ResilienceOptions();

builder.Services.AddHttpClient<IBooksApiClient, BooksApiClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BooksApi:BaseUrl"]!);
        client.Timeout = Timeout.InfiniteTimeSpan;
    })
    .AddResiliencePolicies(booksApiResilience);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

app.UseCommonExceptionHandling();
app.UseCorrelationId();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bupa Books Private API v1"));

app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();
app.MapCommonEndpoints();

app.Run();

public partial class Program
{
}
