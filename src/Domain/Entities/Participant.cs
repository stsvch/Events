using Events.Domain.Exceptions;
using Events.Domain.ValueObjects;

namespace Events.Domain.Entities
{
    public class Participant
    {
        public Guid Id { get; private set; }
        public PersonName Name { get; private set; }
        public EmailAddress Email { get; private set; }
        public DateTimeOffset DateOfBirth { get; private set; }

        private readonly List<EventParticipant> _participations = new();
        public IReadOnlyCollection<EventParticipant> Participations => _participations.AsReadOnly();

        private Participant() { }

        public Participant(PersonName name, EmailAddress email, DateTimeOffset dateOfBirth)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            DateOfBirth = dateOfBirth;
        }
    }
}
