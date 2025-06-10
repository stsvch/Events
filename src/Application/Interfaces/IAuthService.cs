using Events.Application.Common;

namespace Events.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string username, string email, string password);
        Task<AuthResult> LoginAsync(string username, string password);
        Task<AuthResult> RefreshAsync(string refreshToken);
        Task<LogoutResult> LogoutAsync(string refreshToken);
    }

}
