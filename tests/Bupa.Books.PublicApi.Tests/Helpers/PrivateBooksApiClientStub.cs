using Bupa.Books.PublicApi.Models;
using Bupa.Books.PublicApi.Services;

namespace Bupa.Books.PublicApi.Tests.Helpers;

public class PrivateBooksApiClientStub : IPrivateBooksApiClient
{
    public Task<IReadOnlyList<CategorizedBooksDto>> GetCategorizedBooksAsync(
        bool hardcoverOnly, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<CategorizedBooksDto> result =
        [
            new("Adults", [new("Hamlet", "Hardcover"), new("Wuthering Heights", "Paperback")]),
            new("Children", [new("Charlotte's Web", "Paperback")])
        ];
        return Task.FromResult(result);
    }
}
