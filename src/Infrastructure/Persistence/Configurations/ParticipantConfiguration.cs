using Events.Domain.Entities;
using Events.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Persistence.Configurations
{
    public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.ToTable("Participants");
            builder.HasKey(p => p.Id);

            // Вложенный VO PersonName
            builder.OwnsOne(p => p.Name, name =>
            {
                name.Property(n => n.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("FirstName");

                name.Property(n => n.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("LastName");
            });

            // Email VO → строка
            builder.Property(p => p.Email)
                   .HasConversion(
                       vo => vo.Value,
                       str => new EmailAddress(str))
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnName("Email");

            builder.Property(p => p.DateOfBirth)
                   .IsRequired();

            // навигация к EventParticipant
            builder.HasMany(p => p.Participations)    // предполагаем, что в Participant есть ICollection<EventParticipant> Participations
                   .WithOne(ep => ep.Participant)
                   .HasForeignKey(ep => ep.ParticipantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
