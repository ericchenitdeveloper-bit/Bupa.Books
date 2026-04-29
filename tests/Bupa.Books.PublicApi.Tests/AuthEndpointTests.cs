using System.Net;
using System.Net.Http.Json;
using Bupa.Books.PublicApi.Models;
using Bupa.Books.PublicApi.Services;
using Bupa.Books.PublicApi.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Bupa.Books.PublicApi.Tests;

public class AuthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestUsername = "testuser";
    private const string TestPassword = "testpass";

    private readonly WebApplicationFactory<Program> _factory;

    public AuthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Jwt:SecretKey", "test-secret-key-minimum-32-characters!!");
            builder.UseSetting("Jwt:Issuer", "test-issuer");
            builder.UseSetting("Jwt:Audience", "test-audience");
            builder.UseSetting("MockAuth:Username", TestUsername);
            builder.UseSetting("MockAuth:Password", TestPassword);
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPrivateBooksApiClient));
                if (descriptor is not null) services.Remove(descriptor);
                services.AddSingleton<IPrivateBooksApiClient, PrivateBooksApiClientStub>();
            });
        });
    }

    [Fact]
    public async Task GetToken_WithValidCredentials_ReturnsOk()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/token",
            new LoginRequest(TestUsername, TestPassword));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetToken_WithValidCredentials_ReturnsValidToken()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/token",
            new LoginRequest(TestUsername, TestPassword));
        var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
        body!.TokenType.Should().Be("Bearer");
        body.ExpiresIn.Should().Be(3600);
        body.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetToken_WithWrongPassword_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/token",
            new LoginRequest(TestUsername, "wrongpass"));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetToken_WithWrongUsername_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/token",
            new LoginRequest("wronguser", TestPassword));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
