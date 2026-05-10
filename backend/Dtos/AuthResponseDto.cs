namespace MovieWatchlist.Api.Dtos;

public sealed record AuthResponseDto(
    string Token,
    UserMeDto User);
