using System.Net.Http.Json;
using Bupa.Books.PublicApi.Models;

namespace Bupa.Books.PublicApi.Services;

public class PrivateBooksApiClient(HttpClient httpClient) : IPrivateBooksApiClient
{
    public async Task<IReadOnlyList<CategorizedBooksDto>> GetCategorizedBooksAsync(
        bool hardcoverOnly, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            $"api/v1/books?hardcoverOnly={hardcoverOnly}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IReadOnlyList<CategorizedBooksDto>>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Private API returned a null response body.");
    }
}
