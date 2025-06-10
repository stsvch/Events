

namespace Events.Application.DTOs
{
    public class EventImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public DateTimeOffset UploadedAt { get; set; }
    }
}
