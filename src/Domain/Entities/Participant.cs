using Events.Domain.Exceptions;
using Events.Domain.ValueObjects;
using System;

namespace Events.Domain.Entities
{
    public class Participant
    {
        public Guid Id { get; private set; }
        public Guid EventId { get; private set; }
        public PersonName Name { get; private set; }
        public EmailAddress Email { get; private set; }
        public DateTimeOffset DateOfBirth { get; private set; }

        private Participant() { }

        public Participant(Guid eventId, PersonName name, EmailAddress email, DateTimeOffset dateOfBirth)
        {
            Id = Guid.NewGuid();
            EventId = eventId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            DateOfBirth = dateOfBirth;
        }
    }
}
