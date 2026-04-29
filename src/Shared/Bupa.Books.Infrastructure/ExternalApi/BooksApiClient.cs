using System.Net.Http.Json;
using Bupa.Books.Application.Books.Interfaces;
using Bupa.Books.Domain.Entities;

namespace Bupa.Books.Infrastructure.ExternalApi;

public class BooksApiClient(HttpClient httpClient) : IBooksApiClient
{
    public async Task<IReadOnlyList<BookOwner>> GetBookOwnersAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("api/v1/bookowners", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IReadOnlyList<BookOwner>>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Books API returned a null response body.");
    }
}
