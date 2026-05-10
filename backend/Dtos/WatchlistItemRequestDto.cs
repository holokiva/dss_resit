using System.ComponentModel.DataAnnotations;
using MovieWatchlist.Api.Models;

namespace MovieWatchlist.Api.Dtos;

public sealed class WatchlistItemRequestDto
{
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Director { get; set; }

    [Range(1888, 2100)]
    public int? ReleaseYear { get; set; }

    [StringLength(40)]
    public string? Genre { get; set; }

    [EnumDataType(typeof(WatchlistStatus))]
    public WatchlistStatus Status { get; set; }

    [Range(1, 10)]
    public int? Rating { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
