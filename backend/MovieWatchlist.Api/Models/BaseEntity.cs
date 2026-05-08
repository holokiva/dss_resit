namespace MovieWatchlist.Api.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
