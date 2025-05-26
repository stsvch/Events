using Duende.IdentityServer.Models;
using Events.Application.Extensions;
using Events.Application.Interfaces;
using Events.Infrastructure.Identity;
using Events.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Services.Authentication
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityDbContext _db;
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly int _accessMinutes;
        private readonly int _refreshDays;

        public JwtTokenService(
            UserManager<ApplicationUser> userManager,
            IdentityDbContext db,
            IConfiguration config)
        {
            _userManager = userManager;
            _db = db;
            _config = config;

            var js = config.GetSection("JwtSettings");
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(js["Key"]!));
            _accessMinutes = int.Parse(js["AccessTokenDurationMinutes"]!);
            _refreshDays = int.Parse(js["RefreshTokenDurationDays"]!);
        }

        public async Task<JwtAuthenticationResult> GenerateTokensAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return JwtAuthenticationResult.Failure();

            // Собираем claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // Генерируем access token
            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessMinutes),
                signingCredentials: creds);
            string accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Генерируем refresh token
            var refreshValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshEntity = new Events.Infrastructure.Identity.RefreshToken
            {
                Token = refreshValue,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshDays)
            };

            // Удаляем устаревшие и сохраняем новый
            _db.RefreshTokens.RemoveRange(
                _db.RefreshTokens.Where(rt => rt.UserId == user.Id && rt.ExpiresAt <= DateTime.UtcNow));
            await _db.RefreshTokens.AddAsync(refreshEntity);
            await _db.SaveChangesAsync();

            return JwtAuthenticationResult.Success(accessToken, refreshValue);
        }

        public async Task<JwtAuthenticationResult> RefreshTokensAsync(string refreshToken)
        {
            var stored = await _db.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
                return JwtAuthenticationResult.Failure();

            var user = await _userManager.FindByIdAsync(stored.UserId);
            if (user == null) return JwtAuthenticationResult.Failure();

            _db.RefreshTokens.Remove(stored);
            await _db.SaveChangesAsync();

            return await GenerateTokensAsync(user.Id);
        }
    }
}
