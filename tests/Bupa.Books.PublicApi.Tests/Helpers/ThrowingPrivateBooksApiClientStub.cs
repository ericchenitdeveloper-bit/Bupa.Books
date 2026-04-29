using Bupa.Books.PublicApi.Models;
using Bupa.Books.PublicApi.Services;

namespace Bupa.Books.PublicApi.Tests.Helpers;

public class ThrowingPrivateBooksApiClientStub(Exception exception) : IPrivateBooksApiClient
{
    public Task<IReadOnlyList<CategorizedBooksDto>> GetCategorizedBooksAsync(
        bool hardcoverOnly, CancellationToken cancellationToken = default)
        => Task.FromException<IReadOnlyList<CategorizedBooksDto>>(exception);
}
