namespace MovieWatchlist.Api.Models;

public class CommunityMovie : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Director { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public int? ReleaseYear { get; set; }
}
