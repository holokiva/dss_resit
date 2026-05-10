namespace MovieWatchlist.Api.Dtos;

public sealed record CommunityMovieResponseDto(
    Guid Id,
    string Title,
    string? Director,
    string? Genre,
    string? Description,
    int ReleaseYear,
    Guid CreatedByUserId,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
