
namespace Events.Domain.Entities
{
    public class EventParticipant
    {
        public Guid EventId { get; private set; }
        public Event Event { get; private set; }

        public Guid ParticipantId { get; private set; }
        public Participant Participant { get; private set; }

        public DateTimeOffset RegisteredAt { get; private set; }

        private EventParticipant() { }

        public EventParticipant(Guid eventId, Guid participantId)
        {
            EventId = eventId;
            ParticipantId = participantId;
            RegisteredAt = DateTimeOffset.UtcNow;
        }
    }
}
