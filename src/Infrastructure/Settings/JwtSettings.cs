﻿
namespace Events.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int AccessTokenDurationMinutes { get; set; }
        public int RefreshTokenDurationDays { get; set; }
    }
}
