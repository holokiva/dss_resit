using System.ComponentModel.DataAnnotations;
using MovieWatchlist.Api.Models;

namespace MovieWatchlist.Api.Dtos;

public sealed class WatchlistItemRequestDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Range(1888, 2100)]
    public int Year { get; set; }

    [Range(1, 10)]
    public int Rating { get; set; }

    [EnumDataType(typeof(WatchlistStatus))]
    public WatchlistStatus Status { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
