using System.Net;
using Bupa.Books.Application.Books.Interfaces;
using Bupa.Books.PrivateApi.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Bupa.Books.PrivateApi.Tests;

public class BooksEndpointErrorTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestApiKey = "test-api-key";
    private readonly WebApplicationFactory<Program> _factory;

    public BooksEndpointErrorTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
            builder.UseSetting("PrivateApi:ApiKey", TestApiKey));
    }

    private HttpClient CreateClientWithThrowingStub(Exception exception)
    {
        var factory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBooksApiClient));
                if (descriptor is not null) services.Remove(descriptor);
                services.AddSingleton<IBooksApiClient>(new ThrowingBooksApiClientStub(exception));
            }));

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", TestApiKey);
        return client;
    }

    [Fact]
    public async Task GetCategorizedBooks_WhenUpstreamThrowsHttpRequestException_ReturnsBadGateway()
    {
        using var client = CreateClientWithThrowingStub(new HttpRequestException("upstream failed"));
        var response = await client.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
    }

    [Fact]
    public async Task GetCategorizedBooks_WhenCircuitBreakerIsOpen_ReturnsBadGateway()
    {
        using var client = CreateClientWithThrowingStub(new BrokenCircuitException("circuit open"));
        var response = await client.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
    }

    [Fact]
    public async Task GetCategorizedBooks_WhenPollyTimeoutExceeded_ReturnsGatewayTimeout()
    {
        using var client = CreateClientWithThrowingStub(new TimeoutRejectedException("timeout exceeded"));
        var response = await client.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.GatewayTimeout);
    }

    [Fact]
    public async Task GetCategorizedBooks_WhenUnexpectedExceptionOccurs_ReturnsInternalServerError()
    {
        using var client = CreateClientWithThrowingStub(new InvalidOperationException("unexpected"));
        var response = await client.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
