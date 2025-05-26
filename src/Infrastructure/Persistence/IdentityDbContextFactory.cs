using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Events.Infrastructure.Persistence
{
    public class IdentityDbContextFactory
        : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<IdentityDbContext>();
            builder.UseSqlServer(
                config.GetConnectionString("IdentityConnection"));

            var opts =
                new OperationalStoreOptions();

            return new IdentityDbContext(
                builder.Options,
                Options.Create(opts));
        }
    }
}
