namespace Events.WebApi.DTOs.Requests
{
    public class UpdateEventRequest : CreateEventRequest
    {
        public Guid Id { get; set; }
    }
}
