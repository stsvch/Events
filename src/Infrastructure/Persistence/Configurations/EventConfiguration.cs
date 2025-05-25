using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Persistence.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Description)
                   .IsRequired();

            builder.Property(e => e.Venue)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Date)
                   .IsRequired();

            builder.Property(e => e.Capacity)
                   .IsRequired();

            // связь с EventParticipant
            builder.HasMany(e => e.Participants)
                   .WithOne(ep => ep.Event)
                   .HasForeignKey(ep => ep.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            // связь с EventImage — у EventImage нет навигационного свойства Event:
            builder.HasMany(e => e.Images)
                   .WithOne()
                   .HasForeignKey(img => img.EventId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
