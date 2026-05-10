using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWatchlist.Api.Data;
using MovieWatchlist.Api.Dtos;
using MovieWatchlist.Api.Models;

namespace MovieWatchlist.Api.Controllers;

[Authorize]
public sealed class WatchlistController(ApplicationDbContext dbContext) : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WatchlistItemResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<WatchlistItemResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(UnauthorizedProblem());
        }

        var items = await dbContext.WatchlistItems
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => ToResponse(x))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WatchlistItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WatchlistItemResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(UnauthorizedProblem());
        }

        var item = await dbContext.WatchlistItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (item is null)
        {
            return NotFound(NotFoundProblem(id));
        }

        return Ok(ToResponse(item));
    }

    [HttpPost]
    [ProducesResponseType(typeof(WatchlistItemResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WatchlistItemResponseDto>> Create(
        [FromBody] WatchlistItemRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(UnauthorizedProblem());
        }

        var item = new WatchlistItem
        {
            UserId = userId.Value,
            Title = request.Title.Trim(),
            Year = request.Year,
            Rating = request.Rating,
            Status = request.Status,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        dbContext.WatchlistItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = ToResponse(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WatchlistItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WatchlistItemResponseDto>> Update(
        Guid id,
        [FromBody] WatchlistItemRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(UnauthorizedProblem());
        }

        var item = await dbContext.WatchlistItems
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (item is null)
        {
            return NotFound(NotFoundProblem(id));
        }

        item.Title = request.Title.Trim();
        item.Year = request.Year;
        item.Rating = request.Rating;
        item.Status = request.Status;
        item.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        item.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(item));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(UnauthorizedProblem());
        }

        var item = await dbContext.WatchlistItems
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (item is null)
        {
            return NotFound(NotFoundProblem(id));
        }

        dbContext.WatchlistItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static WatchlistItemResponseDto ToResponse(WatchlistItem item) =>
        new(
            item.Id,
            item.Title,
            item.Year,
            item.Rating,
            item.Status,
            item.Notes,
            item.CreatedAtUtc,
            item.UpdatedAtUtc);

    private static ProblemDetails UnauthorizedProblem() =>
        new()
        {
            Title = "Unauthorized",
            Detail = "Access token is invalid.",
            Status = StatusCodes.Status401Unauthorized
        };

    private static ProblemDetails NotFoundProblem(Guid id) =>
        new()
        {
            Title = "Watchlist item not found",
            Detail = $"Item '{id}' was not found.",
            Status = StatusCodes.Status404NotFound
        };
}
