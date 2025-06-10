using Events.Domain.ValueObjects;

namespace Events.Domain.Entities
{
    public class Participant
    {
        public Guid Id { get; private set; }
        public PersonName Name { get; private set; }
        public EmailAddress Email { get; private set; }
        public DateTimeOffset DateOfBirth { get; private set; }

        public string UserId { get; set; }

        private readonly List<EventParticipant> _participations = new();
        public IReadOnlyCollection<EventParticipant> Participations => _participations.AsReadOnly();

        private Participant() { }

        public Participant(PersonName name, EmailAddress email, DateTimeOffset dateOfBirth, string userId)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            DateOfBirth = dateOfBirth;
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
