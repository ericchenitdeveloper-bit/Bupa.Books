using Bupa.Books.PublicApi.Models;

namespace Bupa.Books.PublicApi.Services;

public interface IPrivateBooksApiClient
{
    Task<IReadOnlyList<CategorizedBooksDto>> GetCategorizedBooksAsync(
        bool hardcoverOnly, CancellationToken cancellationToken = default);
}
