using Asp.Versioning;

using Bupa.Books.Application.Books.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bupa.Books.PrivateApi.Controllers;

/// <summary>
/// Internal endpoint that returns book data by owner age group.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BooksController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns all books grouped by age category and sorted alphabetically within each group.
    /// </summary>
    /// <param name="hardcoverOnly">
    /// When <c>true</c>, only books with type <c>Hardcover</c> are included.
    /// Age categories that contain no matching books are omitted entirely.
    /// Defaults to <c>false</c>.
    /// </param>
    /// <returns>
    /// A list of age categories, each containing an alphabetically sorted list of books.
    /// Categories are ordered alphabetically: <c>Adults</c> (owners aged 18+) before <c>Children</c> (owners aged 17 and under).
    /// </returns>
    /// <response code="200">Books successfully retrieved and categorized.</response>
    /// <response code="429">Rate limit exceeded.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategorizedBooksResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetCategorizedBooks(
        [FromQuery] bool hardcoverOnly = false)
    {
        var result = await mediator.Send(new GetCategorizedBooksQuery(hardcoverOnly));
        return Ok(result);
    }
}
