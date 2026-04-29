using System.Net;
using System.Net.Http.Headers;
using Bupa.Books.PublicApi.Services;
using Bupa.Books.PublicApi.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Bupa.Books.PublicApi.Tests;

public class BooksEndpointErrorTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestSecretKey = "test-secret-key-minimum-32-characters!!";
    private const string TestIssuer = "test-issuer";
    private const string TestAudience = "test-audience";

    private readonly WebApplicationFactory<Program> _factory;

    public BooksEndpointErrorTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Jwt:SecretKey", TestSecretKey);
            builder.UseSetting("Jwt:Issuer", TestIssuer);
            builder.UseSetting("Jwt:Audience", TestAudience);
        });
    }

    private HttpClient CreateAuthenticatedClientWithThrowingStub(Exception exception)
    {
        var factory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPrivateBooksApiClient));
                if (descriptor is not null) services.Remove(descriptor);
                services.AddSingleton<IPrivateBooksApiClient>(
                    new ThrowingPrivateBooksApiClientStub(exception));
            }));

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtTokenHelper.Generate(TestSecretKey, TestIssuer, TestAudience));
        return client;
    }

    [Fact]
    public async Task GetCategorizedBooks_WhenPrivateApiThrowsHttpRequestException_ReturnsBadGateway()
    {
        using var client = CreateAuthenticatedClientWithThrowingStub(
            new HttpRequestException("private API unavailable"));
        var response = await client.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
    }

    [Fact]
    public async Task GetCategorizedBooks_WhenUnexpectedExceptionOccurs_ReturnsInternalServerError()
    {
        using var client = CreateAuthenticatedClientWithThrowingStub(
            new InvalidOperationException("unexpected"));
        var response = await client.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
