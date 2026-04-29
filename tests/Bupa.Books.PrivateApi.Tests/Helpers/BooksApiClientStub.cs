using Bupa.Books.Application.Books.Interfaces;
using Bupa.Books.Domain.Entities;

namespace Bupa.Books.PrivateApi.Tests.Helpers;

public class BooksApiClientStub : IBooksApiClient
{
    public Task<IReadOnlyList<BookOwner>> GetBookOwnersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<BookOwner>>(new List<BookOwner>
        {
            new()
            {
                Name = "Jane",
                Age = 23,
                Books =
                [
                    new() { Name = "Hamlet", Type = "Hardcover" },
                    new() { Name = "Wuthering Heights", Type = "Paperback" }
                ]
            },
            new()
            {
                Name = "Charlotte",
                Age = 14,
                Books =
                [
                    new() { Name = "Hamlet", Type = "Paperback" }
                ]
            },
            new()
            {
                Name = "Max",
                Age = 25,
                Books =
                [
                    new() { Name = "Great Expectations", Type = "Hardcover" },
                    new() { Name = "Jane Eyre", Type = "Paperback" }
                ]
            }
        });
    }
}
