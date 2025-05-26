using Duende.IdentityServer.EntityFramework.Options;
using Events.Application.Interfaces;
using Events.Domain.Repositories;
using Events.Infrastructure.Identity;
using Events.Infrastructure.Persistence;
using Events.Infrastructure.Repositories;
using Events.Infrastructure.Repositories.Events.Infrastructure.Repositories;
using Events.Infrastructure.Services.Authentication;
using Events.Infrastructure.Services.Images;
using Events.Infrastructure.Services.Notifications;
using Events.Infrastructure.Settings;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Infrastructure.Extensions
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            // 1) Основной контекст приложения
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // 2) Контекст для Identity и IdentityServer
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("IdentityConnection")));

            // 3) ASP.NET Core Identity с ролями
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

            // 4) Duende IdentityServer: API Authorization
            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, IdentityDbContext>();

            // 5) JWT-аутентификация для API
            services.AddAuthentication()
                .AddIdentityServerJwt();

            // 6) Сервис для генерации и обновления JWT / Refresh токенов
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // 7) Репозитории доменных сущностей
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IParticipantRepository, ParticipantRepository>();
            services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();

            // 8) Настройки внешних сервисов
            services.Configure<SmtpSettings>(config.GetSection("SmtpSettings"));
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

            // 9) Внешние сервисы (SMTP, Cloudinary)
            services.AddTransient<INotificationService, SmtpNotificationService>();
            services.AddTransient<IImageStorageService, CloudinaryImageService>();

            return services;
        }
    }
}
