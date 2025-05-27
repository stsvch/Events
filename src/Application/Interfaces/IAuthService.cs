using Events.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
