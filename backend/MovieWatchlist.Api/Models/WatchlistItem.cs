namespace MovieWatchlist.Api.Models;

public class WatchlistItem : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsWatched { get; set; }
}
