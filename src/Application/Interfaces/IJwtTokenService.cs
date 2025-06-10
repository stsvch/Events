using Events.Application.Extensions;

namespace Events.Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<JwtAuthenticationResult> GenerateTokensAsync(string userId);
        Task<JwtAuthenticationResult> RefreshTokensAsync(string refreshToken);
    }
}
