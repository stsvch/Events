using Events.Domain.Exceptions;
using Events.Domain.ValueObjects;
using System;

namespace Events.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public string Venue { get; private set; }
        public Guid CategoryId { get; private set; }
        public int Capacity { get; private set; }

        private readonly List<Participant> _participants = new();
        public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();

        private readonly List<EventImage> _images = new();
        public IReadOnlyCollection<EventImage> Images => _images.AsReadOnly();

        private Event() { }

        public Event(Guid id, string title, string description, DateTimeOffset date, string venue, Guid categoryId, int capacity)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new InvariantViolationException("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(description))
                throw new InvariantViolationException("Description cannot be empty.");
            if (capacity <= 0)
                throw new InvariantViolationException("Capacity must be positive.");

            Id = id;
            Title = title;
            Description = description;
            Date = date;
            Venue = venue;
            CategoryId = categoryId;
            Capacity = capacity;
        }

        public void AddParticipant(Participant participant)
        {
            if (participant == null)
                throw new ArgumentNullException(nameof(participant));
            if (_participants.Count >= Capacity)
                throw new InvariantViolationException("Event is full.");
            if (_participants.Exists(p => p.Email == participant.Email))
                throw new InvariantViolationException("Participant with this email is already registered.");

            _participants.Add(participant);
        }

        public void RemoveParticipant(Guid participantId)
        {
            var existing = _participants.Find(p => p.Id == participantId);
            if (existing == null)
                throw new EntityNotFoundException(participantId);

            _participants.Remove(existing);
        }

        public void UpdateDetails(string title, string description,
                                  DateTimeOffset date, string venue,
                                  Guid categoryId, int capacity)
        {
            Title = title;
            Description = description;
            Date = date;
            Venue = venue;
            CategoryId = categoryId;
            Capacity = capacity;
        }

        public int ParticipantCount => _participants.Count;

        public void AddImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new InvariantViolationException("Image URL cannot be empty.");

            _images.Add(new EventImage(Id, url, DateTimeOffset.UtcNow));
        }
    }
}