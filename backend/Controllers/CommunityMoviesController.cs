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
            .Join(
                dbContext.Users.AsNoTracking(),
                movie => movie.UserId,
                user => user.Id,
                (movie, user) => new { movie, user })
            .OrderByDescending(x => x.movie.CreatedAtUtc)
            .Select(x => new CommunityMovieResponseDto(
                x.movie.Id,
                x.movie.Title,
                x.movie.Director,
                x.movie.ReleaseYear,
                x.movie.Genre,
                x.movie.Description,
                x.user.Id,
                x.user.DisplayName,
                x.movie.CreatedAtUtc,
                x.movie.UpdatedAtUtc ?? x.movie.CreatedAtUtc))
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
            .Join(
                dbContext.Users.AsNoTracking(),
                movie => movie.UserId,
                user => user.Id,
                (movie, user) => new CommunityMovieResponseDto(
                    movie.Id,
                    movie.Title,
                    movie.Director,
                    movie.ReleaseYear,
                    movie.Genre,
                    movie.Description,
                    user.Id,
                    user.DisplayName,
                    movie.CreatedAtUtc,
                    movie.UpdatedAtUtc ?? movie.CreatedAtUtc))
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

        var displayName = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId.Value)
            .Select(x => x.DisplayName)
            .FirstOrDefaultAsync(cancellationToken) ?? "Unknown";

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ToResponse(item, displayName));
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
        var displayName = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId.Value)
            .Select(x => x.DisplayName)
            .FirstOrDefaultAsync(cancellationToken) ?? "Unknown";

        return Ok(ToResponse(item, displayName));
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

    private static CommunityMovieResponseDto ToResponse(CommunityMovie item, string createdByDisplayName) =>
        new(
            item.Id,
            item.Title,
            item.Director,
            item.ReleaseYear,
            item.Genre,
            item.Description,
            item.UserId,
            createdByDisplayName,
            item.CreatedAtUtc,
            item.UpdatedAtUtc ?? item.CreatedAtUtc);

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
