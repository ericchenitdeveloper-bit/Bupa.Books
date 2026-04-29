using Bupa.Books.Application.Books.Interfaces;
using Bupa.Books.Application.Books.Queries;
using Bupa.Books.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Bupa.Books.Application.Tests.Books.Queries;

public class GetCategorizedBooksQueryHandlerTests
{
    private readonly IBooksApiClient _client = Substitute.For<IBooksApiClient>();
    private readonly GetCategorizedBooksQueryHandler _handler;

    public GetCategorizedBooksQueryHandlerTests()
    {
        _handler = new GetCategorizedBooksQueryHandler(_client);
    }

    [Fact]
    public async Task Handle_GroupsOwnersByAgeCategory()
    {
        _client.GetBookOwnersAsync(Arg.Any<CancellationToken>()).Returns(new List<BookOwner>
        {
            new() { Name = "Alice", Age = 18, Books = [new() { Name = "Book A", Type = "Paperback" }] },
            new() { Name = "Bob",   Age = 17, Books = [new() { Name = "Book B", Type = "Hardcover" }] }
        });

        var result = await _handler.Handle(new GetCategorizedBooksQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(c => c.AgeCategory == "Adults");
        result.Should().Contain(c => c.AgeCategory == "Children");
    }

    [Fact]
    public async Task Handle_SortsBooksAlphabeticallyWithinCategory()
    {
        _client.GetBookOwnersAsync(Arg.Any<CancellationToken>()).Returns(new List<BookOwner>
        {
            new() { Name = "Alice", Age = 25, Books =
            [
                new() { Name = "Zebra Tales", Type = "Paperback" },
                new() { Name = "Apple Stories", Type = "Paperback" },
                new() { Name = "Mango Diaries", Type = "Paperback" }
            ]}
        });

        var result = await _handler.Handle(new GetCategorizedBooksQuery(), CancellationToken.None);

        var books = result.Single(c => c.AgeCategory == "Adults").Books;
        books.Select(b => b.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Handle_CategoriesReturnedAlphabetically()
    {
        _client.GetBookOwnersAsync(Arg.Any<CancellationToken>()).Returns(new List<BookOwner>
        {
            new() { Name = "Child", Age = 10, Books = [new() { Name = "A", Type = "Paperback" }] },
            new() { Name = "Adult", Age = 30, Books = [new() { Name = "B", Type = "Paperback" }] }
        });

        var result = await _handler.Handle(new GetCategorizedBooksQuery(), CancellationToken.None);

        result[0].AgeCategory.Should().Be("Adults");
        result[1].AgeCategory.Should().Be("Children");
    }

    [Theory]
    [InlineData(18, "Adults")]
    [InlineData(17, "Children")]
    [InlineData(0,  "Children")]
    [InlineData(100, "Adults")]
    public async Task Handle_AgeBoundary_CategorizesCorrectly(int age, string expectedCategory)
    {
        _client.GetBookOwnersAsync(Arg.Any<CancellationToken>()).Returns(new List<BookOwner>
        {
            new() { Name = "Owner", Age = age, Books = [new() { Name = "A Book", Type = "Paperback" }] }
        });

        var result = await _handler.Handle(new GetCategorizedBooksQuery(), CancellationToken.None);

        result.Should().ContainSingle(c => c.AgeCategory == expectedCategory);
    }

    [Fact]
    public async Task Handle_HardcoverOnly_FiltersToHardcoverBooks()
    {
        _client.GetBookOwnersAsync(Arg.Any<CancellationToken>()).Returns(new List<BookOwner>
        {
            new() { Name = "Alice", Age = 25, Books =
            [
                new() { Name = "Hard Book", Type = "Hardcover" },
                new() { Name = "Soft Book", Type = "Paperback" }
            ]}
        });

        var result = await _handler.Handle(new GetCategorizedBooksQuery(HardcoverOnly: true), CancellationToken.None);

        var books = result.Single(c => c.AgeCategory == "Adults").Books;
        books.Should().ContainSingle(b => b.Name == "Hard Book");
        books.Should().NotContain(b => b.Name == "Soft Book");
    }

    [Fact]
    public async Task Handle_HardcoverOnly_ExcludesCategoryWithNoHardcoverBooks()
    {
        _client.GetBookOwnersAsync(Arg.Any<CancellationToken>()).Returns(new List<BookOwner>
        {
            new() { Name = "Adult", Age = 25, Books = [new() { Name = "Hard", Type = "Hardcover" }] },
            new() { Name = "Child", Age = 10, Books = [new() { Name = "Soft", Type = "Paperback" }] }
        });

        var result = await _handler.Handle(new GetCategorizedBooksQuery(HardcoverOnly: true), CancellationToken.None);

        result.Should().ContainSingle(c => c.AgeCategory == "Adults");
        result.Should().NotContain(c => c.AgeCategory == "Children");
    }

    [Fact]
    public async Task Handle_HardcoverOnly_IsCaseInsensitive()
    {
        _client.GetBookOwnersAsync(Arg.Any<CancellationToken>()).Returns(new List<BookOwner>
        {
            new() { Name = "Alice", Age = 25, Books = [new() { Name = "Book", Type = "hardcover" }] }
        });

        var result = await _handler.Handle(new GetCategorizedBooksQuery(HardcoverOnly: true), CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Books.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_WhenNoOwners_ReturnsEmpty()
    {
        _client.GetBookOwnersAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<BookOwner>());

        var result = await _handler.Handle(new GetCategorizedBooksQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MultipleOwnersInSameCategory_AggregatesBooks()
    {
        _client.GetBookOwnersAsync(Arg.Any<CancellationToken>()).Returns(new List<BookOwner>
        {
            new() { Name = "Adult1", Age = 30, Books = [new() { Name = "Book C", Type = "Paperback" }] },
            new() { Name = "Adult2", Age = 40, Books = [new() { Name = "Book A", Type = "Paperback" }] }
        });

        var result = await _handler.Handle(new GetCategorizedBooksQuery(), CancellationToken.None);

        var adults = result.Single(c => c.AgeCategory == "Adults");
        adults.Books.Should().HaveCount(2);
        adults.Books[0].Name.Should().Be("Book A");
        adults.Books[1].Name.Should().Be("Book C");
    }
}
