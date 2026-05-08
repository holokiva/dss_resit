using MovieWatchlist.Api.Dtos;

namespace MovieWatchlist.Api.Services;

public interface IJwtTokenService
{
    AuthResultDto GenerateToken(Guid userId, string email, string displayName);
}
