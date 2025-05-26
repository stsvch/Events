using Duende.IdentityServer.EntityFramework.Options;
using Events.Application.Interfaces;
using Events.Domain.Repositories;
using Events.Infrastructure.Identity;
using Events.Infrastructure.Persistence;
using Events.Infrastructure.Repositories;
using Events.Infrastructure.Services.Authentication;
using Events.Infrastructure.Services.Caching;
using Events.Infrastructure.Services.Images;
using Events.Infrastructure.Services.Notifications;
using Events.Infrastructure.Settings;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

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

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, IdentityDbContext>()
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(config.GetConnectionString("IdentityConnection"));
                });

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IParticipantRepository, ParticipantRepository>();
            services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();

            services.Configure<SmtpSettings>(config.GetSection("SmtpSettings"));
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            services.AddTransient<INotificationService, SmtpNotificationService>();
            services.AddTransient<CloudinaryImageService>();
            services.AddTransient<IImageStorageService>(sp =>
                new CachedImageService(
                    sp.GetRequiredService<CloudinaryImageService>(),
                    sp.GetRequiredService<IDistributedCache>()));


            return services;
        }
    }
}
