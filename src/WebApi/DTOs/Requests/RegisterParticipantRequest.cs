namespace Events.WebApi.DTOs.Requests
{
    public class RegisterParticipantRequest
    {
        public Guid EventId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
