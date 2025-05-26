using Events.Application.Interfaces;
using Events.Infrastructure.Identity;
using Events.WebApi.DTOs.Requests;
using Events.WebApi.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Events.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtSvc;
        private readonly SignInManager<ApplicationUser> _signInMgr;
        private readonly UserManager<ApplicationUser> _userMgr;

        public AuthController(
            IJwtTokenService jwtSvc,
            SignInManager<ApplicationUser> signInMgr,
            UserManager<ApplicationUser> userMgr)
        {
            _jwtSvc = jwtSvc;
            _signInMgr = signInMgr;
            _userMgr = userMgr;
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userMgr.FindByNameAsync(req.Username);
            if (user == null) return Unauthorized();

            var res = await _signInMgr.CheckPasswordSignInAsync(user, req.Password, false);
            if (!res.Succeeded) return Unauthorized();

            // Вызываем application-слой, получаем JwtAuthenticationResult
            var authResult = await _jwtSvc.GenerateTokensAsync(user.Id);
            if (!authResult.Succeeded) return StatusCode(500);

            // Мапим в HTTP DTO
            var response = new TokenResponse
            {
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken
            };
            return Ok(response);
        }

        [HttpPost("refresh"), AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
        {
            var authResult = await _jwtSvc.RefreshTokensAsync(req.RefreshToken);
            if (!authResult.Succeeded) return Unauthorized();

            var response = new TokenResponse
            {
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken
            };
            return Ok(response);
        }
    }
}
