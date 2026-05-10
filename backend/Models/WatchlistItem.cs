namespace MovieWatchlist.Api.Models;

public class WatchlistItem : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Director { get; set; }
    public int? ReleaseYear { get; set; }
    public string? Genre { get; set; }
    public WatchlistStatus Status { get; set; } = WatchlistStatus.PlanToWatch;
    public int? Rating { get; set; }
    public string? Notes { get; set; }
}
