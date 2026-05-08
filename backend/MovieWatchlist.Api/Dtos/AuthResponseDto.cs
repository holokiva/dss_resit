namespace MovieWatchlist.Api.Dtos;

public sealed record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    UserMeDto User);
