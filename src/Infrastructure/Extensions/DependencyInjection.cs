using System.Text;
using Duende.IdentityServer.EntityFramework.Options;
using Events.Application.Interfaces;
using Events.Domain.Repositories;
using Events.Infrastructure.Identity;
using Events.Infrastructure.Persistence;
using Events.Infrastructure.Repositories;
using Events.Infrastructure.Services.Authentication;
using Events.Infrastructure.Services.Images;
using Events.Infrastructure.Services.Notifications;
using Events.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Events.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information));

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("IdentityConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
            var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>()!;

            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);
            var issuer = jwtSettings.Issuer;
            var audience = jwtSettings.Audience;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateLifetime = true
                };
            });

            services.AddAuthorization();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IEventImageRepository, EventImageRepository>();

            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IParticipantRepository, ParticipantRepository>();

            services.Configure<SmtpSettings>(config.GetSection("SmtpSettings"));
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            services.AddTransient<INotificationService, SmtpNotificationService>();

            services.AddTransient<IImageStorageService,CloudinaryImageService>();

            return services;
        }
    }
}
