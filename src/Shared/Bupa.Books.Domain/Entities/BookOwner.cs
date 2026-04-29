namespace Bupa.Books.Domain.Entities;

public class BookOwner
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public List<Book> Books { get; set; } = [];
}
