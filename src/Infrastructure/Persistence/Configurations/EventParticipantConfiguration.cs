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
    public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
    {
        public void Configure(EntityTypeBuilder<EventParticipant> builder)
        {
            builder.ToTable("EventParticipants");

            builder.HasKey(ep => new { ep.EventId, ep.ParticipantId });

            builder.HasOne(ep => ep.Event)
                   .WithMany(e => e.Participants)
                   .HasForeignKey(ep => ep.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ep => ep.Participant)
                   .WithMany(p => p.Participations)    
                   .HasForeignKey(ep => ep.ParticipantId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ep => ep.RegisteredAt)
                   .IsRequired();
        }
    }
}
