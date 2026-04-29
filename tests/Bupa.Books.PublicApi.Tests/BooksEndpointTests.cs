using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bupa.Books.PublicApi.Models;
using Bupa.Books.PublicApi.Services;
using Bupa.Books.PublicApi.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Bupa.Books.PublicApi.Tests;

public class BooksEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestSecretKey = "test-secret-key-minimum-32-characters!!";
    private const string TestIssuer = "test-issuer";
    private const string TestAudience = "test-audience";

    private readonly WebApplicationFactory<Program> _factory;

    public BooksEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Jwt:SecretKey", TestSecretKey);
            builder.UseSetting("Jwt:Issuer", TestIssuer);
            builder.UseSetting("Jwt:Audience", TestAudience);
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPrivateBooksApiClient));
                if (descriptor is not null) services.Remove(descriptor);
                services.AddSingleton<IPrivateBooksApiClient, PrivateBooksApiClientStub>();
            });
        });
    }

    private HttpClient CreateAuthenticatedClient()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtTokenHelper.Generate(TestSecretKey, TestIssuer, TestAudience));
        return client;
    }

    [Fact]
    public async Task GetCategorizedBooks_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCategorizedBooks_WithValidToken_ReturnsOk()
    {
        using var client = CreateAuthenticatedClient();
        var response = await client.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCategorizedBooks_ReturnsExpectedCategories()
    {
        using var client = CreateAuthenticatedClient();
        var result = await client.GetFromJsonAsync<List<CategorizedBooksDto>>("/api/v1/books");
        result.Should().NotBeNull();
        result!.Should().Contain(c => c.AgeCategory == "Adults");
        result.Should().Contain(c => c.AgeCategory == "Children");
    }

    [Fact]
    public async Task GetCategorizedBooks_ResponseIncludesCorrelationId()
    {
        using var client = CreateAuthenticatedClient();
        var response = await client.GetAsync("/api/v1/books");
        response.Headers.Should().ContainKey("X-Correlation-ID");
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
