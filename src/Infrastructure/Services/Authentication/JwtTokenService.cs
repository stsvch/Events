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
            // 1. Получаем пользователя
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return JwtAuthenticationResult.Failure();

            // 2. Собираем claims для access-токена
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // 3. Генерация access-токена
            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessMinutes),
                signingCredentials: creds);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            // 4. Пакетно удаляем просроченные refresh-токены одним SQL-запросом
            await _db.RefreshTokens
                     .Where(rt => rt.UserId == userId && rt.ExpiresAt <= DateTime.UtcNow)
                     .ExecuteDeleteAsync();

            // 5. Генерируем новый refresh-токен и сохраняем
            var refreshValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshEntity = new RefreshToken
            {
                Token = refreshValue,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshDays)
            };

            await _db.RefreshTokens.AddAsync(refreshEntity);
            await _db.SaveChangesAsync();

            return JwtAuthenticationResult.Success(accessToken, refreshValue);
        }

        public async Task<JwtAuthenticationResult> RefreshTokensAsync(string refreshToken)
        {
            // 1. Сначала находим существующую запись, чтобы получить userId
            var stored = await _db.RefreshTokens
                                  .AsNoTracking()
                                  .SingleOrDefaultAsync(rt => rt.Token == refreshToken);
            if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
                return JwtAuthenticationResult.Failure();

            var userId = stored.UserId;

            // 2. Удаляем этот refresh-токен пакетно (одной командой)
            await _db.RefreshTokens
                     .Where(rt => rt.Token == refreshToken)
                     .ExecuteDeleteAsync();

            // 3. Генерируем новый комплект токенов
            return await GenerateTokensAsync(userId);
        }
    }
}
