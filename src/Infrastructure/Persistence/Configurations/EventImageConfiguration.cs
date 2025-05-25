using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.Infrastructure.Persistence.Configurations
{
    public class EventImageConfiguration : IEntityTypeConfiguration<EventImage>
    {
        public void Configure(EntityTypeBuilder<EventImage> builder)
        {
            builder.ToTable("EventImages");
            builder.HasKey(img => img.Id);

            builder.Property(img => img.Url)
                   .IsRequired();

            builder.Property(img => img.UploadedAt)
                   .IsRequired();
        }
    }
}
