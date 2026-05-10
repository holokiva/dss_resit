using MovieWatchlist.Api.Models;

namespace MovieWatchlist.Api.Dtos;

public sealed record WatchlistItemResponseDto(
    Guid Id,
    string Title,
    int Year,
    int Rating,
    WatchlistStatus Status,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
