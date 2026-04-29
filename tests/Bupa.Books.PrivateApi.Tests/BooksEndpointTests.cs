using System.Net;
using System.Net.Http.Json;

using Bupa.Books.Application.Books.Interfaces;
using Bupa.Books.Application.Books.Queries;
using Bupa.Books.PrivateApi.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Bupa.Books.PrivateApi.Tests;

public class BooksEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestApiKey = "test-api-key";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BooksEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("PrivateApi:ApiKey", TestApiKey);
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBooksApiClient));
                if (descriptor is not null) services.Remove(descriptor);
                services.AddSingleton<IBooksApiClient, BooksApiClientStub>();
            });
        });

        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Api-Key", TestApiKey);
    }

    [Fact]
    public async Task GetCategorizedBooks_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCategorizedBooks_WithoutApiKey_ReturnsUnauthorized()
    {
        using var unauthClient = _factory.CreateClient();
        var response = await unauthClient.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCategorizedBooks_WithWrongApiKey_ReturnsUnauthorized()
    {
        using var wrongKeyClient = _factory.CreateClient();
        wrongKeyClient.DefaultRequestHeaders.Add("X-Api-Key", "wrong-key");
        var response = await wrongKeyClient.GetAsync("/api/v1/books");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCategorizedBooks_ReturnsAdultsAndChildren()
    {
        var result = await _client.GetFromJsonAsync<List<CategorizedBooksResponse>>("/api/v1/books");

        result.Should().NotBeNull();
        result!.Should().Contain(c => c.AgeCategory == "Adults");
        result!.Should().Contain(c => c.AgeCategory == "Children");
    }

    [Fact]
    public async Task GetCategorizedBooks_ReturnsBooksAlphabeticallySorted()
    {
        var result = await _client.GetFromJsonAsync<List<CategorizedBooksResponse>>("/api/v1/books");

        foreach (var category in result!)
            category.Books.Select(b => b.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetCategorizedBooks_CategoriesAreInAlphabeticalOrder()
    {
        var result = await _client.GetFromJsonAsync<List<CategorizedBooksResponse>>("/api/v1/books");

        result!.Select(c => c.AgeCategory).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetCategorizedBooks_WithHardcoverFilter_ReturnsOnlyHardcoverBooks()
    {
        var result = await _client.GetFromJsonAsync<List<CategorizedBooksResponse>>("/api/v1/books?hardcoverOnly=true");

        result.Should().NotBeNull();
        result!.SelectMany(c => c.Books).Should().AllSatisfy(b =>
            b.Type.Should().BeEquivalentTo("Hardcover"));
    }

    [Fact]
    public async Task GetCategorizedBooks_WithHardcoverFilter_ExcludesEmptyCategories()
    {
        var result = await _client.GetFromJsonAsync<List<CategorizedBooksResponse>>("/api/v1/books?hardcoverOnly=true");

        result.Should().NotBeNull();
        result!.Should().AllSatisfy(c => c.Books.Should().NotBeEmpty());
    }

    [Fact]
    public async Task GetCategorizedBooks_ResponseIncludesCorrelationId()
    {
        var response = await _client.GetAsync("/api/v1/books");
        response.Headers.Should().ContainKey("X-Correlation-ID");
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
