using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EventImage> EventImages { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
