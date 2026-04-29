using Bupa.Books.Application.Books.Interfaces;
using Bupa.Books.Domain.Entities;

namespace Bupa.Books.PrivateApi.Tests.Helpers;

public class ThrowingBooksApiClientStub(Exception exception) : IBooksApiClient
{
    public Task<IReadOnlyList<BookOwner>> GetBookOwnersAsync(CancellationToken cancellationToken = default)
        => Task.FromException<IReadOnlyList<BookOwner>>(exception);
}
