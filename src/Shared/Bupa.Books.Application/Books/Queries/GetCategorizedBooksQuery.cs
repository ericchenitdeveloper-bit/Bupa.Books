using MediatR;

namespace Bupa.Books.Application.Books.Queries;

public record GetCategorizedBooksQuery(bool HardcoverOnly = false) : IRequest<IReadOnlyList<CategorizedBooksResponse>>;
