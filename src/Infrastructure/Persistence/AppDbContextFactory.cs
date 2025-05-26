using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1) Считываем appsettings.json из проекта WebApi (или Infrastructure)
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  // при design-time cwd будет путь к Infrastructure
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // 2) Строим options для контекста
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connStr = config.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connStr);

            return new AppDbContext(builder.Options);
        }
    }
}
