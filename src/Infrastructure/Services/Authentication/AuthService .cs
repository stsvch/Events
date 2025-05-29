using Events.Application.Common;
using Events.Application.Interfaces;
using Events.Infrastructure.Identity;
using Events.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IdentityDbContext _db;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService, IdentityDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _db = db;
        }
        public async Task<AuthResult> RegisterAsync(string username, string email, string password)
        {
            if (await _userManager.FindByNameAsync(username) != null)
                return AuthResult.Failure("Username is already taken.");

            if (await _userManager.FindByEmailAsync(email) != null)
                return AuthResult.Failure("Email is already in use.");

            var user = new ApplicationUser { UserName = username, Email = email };
            var res = await _userManager.CreateAsync(user, password);
            if (!res.Succeeded)
            {
                var errors = res.Errors.Select(e => e.Description).ToArray();
                return AuthResult.Failure(errors);
            }

            var tokens = await _jwtTokenService.GenerateTokensAsync(user.Id);
            return AuthResult.Success(tokens.AccessToken, tokens.RefreshToken, user.Id);
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return AuthResult.Failure(new[] { "Invalid username or password." });
            }

            var chk = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!chk.Succeeded)
            {
                return AuthResult.Failure(new[] { "Invalid username or password." });
            }

            var tokens = await _jwtTokenService.GenerateTokensAsync(user.Id);
            return AuthResult.Success(tokens.AccessToken, tokens.RefreshToken, user.Id);
        }

        public async Task<AuthResult> RefreshAsync(string refreshToken)
        {
            var jwtRes = await _jwtTokenService.RefreshTokensAsync(refreshToken);
            if (!jwtRes.Succeeded)
            {
                return AuthResult.Failure(new[] { "Invalid or expired refresh token." });
            }

            return AuthResult.Success(jwtRes.AccessToken, jwtRes.RefreshToken, string.Empty);
        }

        public async Task<LogoutResult> LogoutAsync(string refreshToken)
        {
            var stored = await _db.RefreshTokens
                                  .SingleOrDefaultAsync(t => t.Token == refreshToken);
            if (stored == null)
            {
                return new LogoutResult
                {
                    Succeeded = false,
                    Errors = new[] { "Refresh token not found." }
                };
            }

            _db.RefreshTokens.Remove(stored);
            await _db.SaveChangesAsync();

            return new LogoutResult { Succeeded = true };
        }

    }
}
