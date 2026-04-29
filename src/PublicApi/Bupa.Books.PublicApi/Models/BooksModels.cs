namespace Bupa.Books.PublicApi.Models;

/// <summary>A single book title and its binding type.</summary>
/// <param name="Name">The title of the book.</param>
/// <param name="Type">The binding type, e.g. <c>Hardcover</c> or <c>Paperback</c>.</param>
public record BookDto(string Name, string Type);

/// <summary>
/// A collection of books grouped under an owner age category heading.
/// </summary>
/// <param name="AgeCategory">
/// Either <c>Adults</c> (owners aged 18 and above) or <c>Children</c> (owners aged 17 and under).
/// </param>
/// <param name="Books">Books belonging to this age category, sorted alphabetically by title.</param>
public record CategorizedBooksDto(string AgeCategory, IReadOnlyList<BookDto> Books);
