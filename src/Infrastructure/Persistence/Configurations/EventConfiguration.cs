using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

            builder.HasMany(e => e.Participants)
                   .WithOne(ep => ep.Event)
                   .HasForeignKey(ep => ep.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Images)
                   .WithOne()
                   .HasForeignKey(img => img.EventId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
