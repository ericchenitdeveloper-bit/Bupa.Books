using Asp.Versioning;
using Bupa.Books.PublicApi.Models;
using Bupa.Books.PublicApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bupa.Books.PublicApi.Controllers;

/// <summary>
/// Provides access to book data categorized by owner age group.
/// </summary>
/// <remarks>
/// All endpoints require a valid JWT bearer token.
///Authorization: Bearer {token}
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class BooksController(IPrivateBooksApiClient privateApiClient) : ControllerBase
{
    /// <summary>
    /// Returns all books grouped by age category and sorted alphabetically within each group.
    /// </summary>
    /// <param name="hardcoverOnly">
    /// When <c>true</c>, only books with type <c>Hardcover</c> are included.
    /// Age categories that contain no matching books are omitted from the response.
    /// Defaults to <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>
    /// A list of age categories, each containing an alphabetically sorted list of books.
    /// Categories are ordered alphabetically — <c>Adults</c> (owners aged 18 and above)
    /// before <c>Children</c> (owners aged 17 and under).
    /// </returns>
    /// <response code="200">Books successfully retrieved and categorized.</response>
    /// <response code="401">Missing, expired, or invalid JWT bearer token.</response>
    /// <response code="429">Too many requests — rate limit exceeded. Retry after 1 minute.</response>
    /// <response code="502">The upstream data service returned an error.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategorizedBooksDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetCategorizedBooks(
        [FromQuery] bool hardcoverOnly = false,
        CancellationToken cancellationToken = default)
    {
        var result = await privateApiClient.GetCategorizedBooksAsync(hardcoverOnly, cancellationToken);
        return Ok(result);
    }
}
