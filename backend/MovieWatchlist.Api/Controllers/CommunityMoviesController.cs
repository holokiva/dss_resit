using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWatchlist.Api.Data;
using MovieWatchlist.Api.Dtos;
using MovieWatchlist.Api.Models;

namespace MovieWatchlist.Api.Controllers;

[Route("api/community-movies")]
public sealed class CommunityMoviesController(ApplicationDbContext dbContext) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CommunityMovieResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CommunityMovieResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.CommunityMovies
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(ToResponseExpression)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CommunityMovieResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommunityMovieResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await dbContext.CommunityMovies
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(ToResponseExpression)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return NotFound(NotFoundProblem(id));
        }

        return Ok(item);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(CommunityMovieResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CommunityMovieResponseDto>> Create(
        [FromBody] CommunityMovieRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(UnauthorizedProblem());
        }

        var item = new CommunityMovie
        {
            UserId = userId.Value,
            Title = request.Title.Trim(),
            Director = Normalize(request.Director),
            Genre = Normalize(request.Genre),
            Description = Normalize(request.Description),
            ReleaseYear = request.ReleaseYear
        };

        dbContext.CommunityMovies.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ToResponse(item));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CommunityMovieResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommunityMovieResponseDto>> Update(
        Guid id,
        [FromBody] CommunityMovieRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(UnauthorizedProblem());
        }

        var item = await dbContext.CommunityMovies
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (item is null)
        {
            return NotFound(NotFoundProblem(id));
        }

        if (item.UserId != userId.Value)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ForbiddenProblem());
        }

        item.Title = request.Title.Trim();
        item.Director = Normalize(request.Director);
        item.Genre = Normalize(request.Genre);
        item.Description = Normalize(request.Description);
        item.ReleaseYear = request.ReleaseYear;
        item.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ToResponse(item));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(UnauthorizedProblem());
        }

        var item = await dbContext.CommunityMovies
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (item is null)
        {
            return NotFound(NotFoundProblem(id));
        }

        if (item.UserId != userId.Value)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ForbiddenProblem());
        }

        dbContext.CommunityMovies.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static readonly System.Linq.Expressions.Expression<Func<CommunityMovie, CommunityMovieResponseDto>> ToResponseExpression =
        x => new CommunityMovieResponseDto(
            x.Id,
            x.Title,
            x.Director,
            x.Genre,
            x.Description,
            x.ReleaseYear,
            x.UserId,
            x.CreatedAtUtc,
            x.UpdatedAtUtc);

    private static CommunityMovieResponseDto ToResponse(CommunityMovie item) =>
        new(
            item.Id,
            item.Title,
            item.Director,
            item.Genre,
            item.Description,
            item.ReleaseYear,
            item.UserId,
            item.CreatedAtUtc,
            item.UpdatedAtUtc);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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
            Title = "Community movie not found",
            Detail = $"Movie '{id}' was not found.",
            Status = StatusCodes.Status404NotFound
        };

    private static ProblemDetails ForbiddenProblem() =>
        new()
        {
            Title = "Forbidden",
            Detail = "You do not own this community movie.",
            Status = StatusCodes.Status403Forbidden
        };
}
