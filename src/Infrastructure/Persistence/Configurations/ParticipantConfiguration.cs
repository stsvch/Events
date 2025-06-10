using Events.Domain.Entities;
using Events.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.Infrastructure.Persistence.Configurations
{
    public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.ToTable("Participants");
            builder.HasKey(p => p.Id);

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

            builder.Property(p => p.Email)
                   .HasConversion(
                       vo => vo.Value,
                       str => new EmailAddress(str))
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnName("Email");

            builder.Property(p => p.DateOfBirth)
                   .IsRequired();

            builder.HasMany(p => p.Participations)   
                   .WithOne(ep => ep.Participant)
                   .HasForeignKey(ep => ep.ParticipantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
