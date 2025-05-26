using Events.Application.Common;
using Events.Application.Interfaces;
using Events.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;


namespace Events.Infrastructure.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResult> RegisterAsync(string username, string email, string password)
        {
            var user = new ApplicationUser { UserName = username, Email = email };
            var res = await _userManager.CreateAsync(user, password);
            if (!res.Succeeded)
                return AuthResult.Failure();

            // передаём user.Id, а не объект user
            var tokens = await _jwtTokenService.GenerateTokensAsync(user.Id);
            return AuthResult.Success(tokens.AccessToken, tokens.RefreshToken);
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return AuthResult.Failure();

            var chk = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!chk.Succeeded)
                return AuthResult.Failure();

            // передаём user.Id
            var tokens = await _jwtTokenService.GenerateTokensAsync(user.Id);
            return AuthResult.Success(tokens.AccessToken, tokens.RefreshToken);
        }

        public async Task<AuthResult> RefreshAsync(string refreshToken)
        {
            // один строковый аргумент
            var jwtRes = await _jwtTokenService.RefreshTokensAsync(refreshToken);

            return jwtRes.Succeeded
                ? AuthResult.Success(jwtRes.AccessToken, jwtRes.RefreshToken)
                : AuthResult.Failure();
        }
    }
}
