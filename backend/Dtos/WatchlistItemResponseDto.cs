using MovieWatchlist.Api.Models;

namespace MovieWatchlist.Api.Dtos;

public sealed record WatchlistItemResponseDto(
    Guid Id,
    string Title,
    string? Director,
    int? ReleaseYear,
    string? Genre,
    WatchlistStatus Status,
    int? Rating,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt);
