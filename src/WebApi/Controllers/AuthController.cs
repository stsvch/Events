using Events.Application.DTOs;
using Events.Application.Interfaces;
using Events.WebApi.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
            => _auth = auth;

        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest req)
        {
            var res = await _auth.RegisterAsync(req.Username, req.Email, req.Password);
            if (!res.Succeeded) return BadRequest("Registration failed");
            return Ok(new TokenDto
            {
                AccessToken = res.AccessToken,
                RefreshToken = res.RefreshToken
            });
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var res = await _auth.LoginAsync(req.Username, req.Password);
            if (!res.Succeeded) return Unauthorized("Invalid credentials");
            return Ok(new TokenDto
            {
                AccessToken = res.AccessToken,
                RefreshToken = res.RefreshToken
            });
        }

        [HttpPost("refresh"), AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
        {
            var res = await _auth.RefreshAsync(req.RefreshToken);
            if (!res.Succeeded) return Unauthorized("Invalid or expired refresh token");
            return Ok(new TokenDto
            {
                AccessToken = res.AccessToken,
                RefreshToken = res.RefreshToken
            });
        }
    }
}
