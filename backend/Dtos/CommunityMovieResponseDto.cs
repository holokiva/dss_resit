namespace MovieWatchlist.Api.Dtos;

public sealed record CommunityMovieResponseDto(
    Guid Id,
    string Title,
    string? Director,
    int? ReleaseYear,
    string? Genre,
    string? Description,
    Guid CreatedByUserId,
    string CreatedByDisplayName,
    DateTime CreatedAt,
    DateTime UpdatedAt);
