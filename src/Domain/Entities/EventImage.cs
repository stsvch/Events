using Events.Domain.Exceptions;

namespace Events.Domain.Entities
{
    public  class EventImage
    {
        public Guid Id { get; private set; }
        public Guid EventId { get; private set; }
        public string Url { get; private set; }
        public DateTimeOffset UploadedAt { get; private set; }

        private EventImage() { }

        public EventImage(Guid eventId, string url, DateTimeOffset uploadedAt)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvariantViolationException("URL cannot be empty.");
            }
            Id = Guid.NewGuid();
            EventId = eventId;
            Url = url;
            UploadedAt = uploadedAt;
        }
    }
}