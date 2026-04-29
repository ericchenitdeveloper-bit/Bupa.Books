using Bupa.Books.Domain.Entities;

namespace Bupa.Books.Application.Books.Interfaces;

public interface IBooksApiClient
{
    Task<IReadOnlyList<BookOwner>> GetBookOwnersAsync(CancellationToken cancellationToken = default);
}
