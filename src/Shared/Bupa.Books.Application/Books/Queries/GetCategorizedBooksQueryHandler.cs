using Bupa.Books.Application.Books.Interfaces;
using MediatR;

namespace Bupa.Books.Application.Books.Queries;

public class GetCategorizedBooksQueryHandler(IBooksApiClient booksApiClient)
    : IRequestHandler<GetCategorizedBooksQuery, IReadOnlyList<CategorizedBooksResponse>>
{
    private const int AdultAge = 18;

    public async Task<IReadOnlyList<CategorizedBooksResponse>> Handle(
        GetCategorizedBooksQuery request,
        CancellationToken cancellationToken)
    {
        var owners = await booksApiClient.GetBookOwnersAsync(cancellationToken);

        return owners
            .GroupBy(owner => owner.Age >= AdultAge ? "Adults" : "Children")
            .OrderBy(g => g.Key)
            .Select(g => new CategorizedBooksResponse(
                g.Key,
                g.SelectMany(owner => owner.Books)
                    .Where(book => !request.HardcoverOnly
                                   || book.Type.Equals("Hardcover", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(book => book.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(book => new BookResponse(book.Name, book.Type))
                    .ToList()))
            .Where(c => c.Books.Any())
            .ToList();
    }
}
