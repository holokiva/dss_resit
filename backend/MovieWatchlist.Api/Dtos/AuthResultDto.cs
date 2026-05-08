namespace MovieWatchlist.Api.Dtos;

public record AuthResultDto(string AccessToken, DateTime ExpiresAtUtc);
