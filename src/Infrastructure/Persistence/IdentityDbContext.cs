using Duende.IdentityServer.EntityFramework.Options;
using Duende.IdentityServer.Models;
using Events.Infrastructure.Identity;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace Events.Infrastructure.Persistence
{
    public class IdentityDbContext :
        ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
        public IdentityDbContext(
            DbContextOptions<IdentityDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
