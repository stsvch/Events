namespace Events.WebApi.DTOs.Requests
{
    public class CreateEventRequest
    {
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public string Venue { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }

    }
}
