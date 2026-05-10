namespace MovieWatchlist.Api.Models;

public class WatchlistItem : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Rating { get; set; }
    public WatchlistStatus Status { get; set; } = WatchlistStatus.Planned;
    public string? Notes { get; set; }
}
