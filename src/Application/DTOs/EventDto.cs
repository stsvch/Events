

namespace Events.Application.DTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Venue { get; set; }
        public Guid CategoryId { get; set; }
        public string Availability { get; set; }
    }
}
