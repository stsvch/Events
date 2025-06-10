using Events.Domain.Exceptions;
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

        private readonly List<EventParticipant> _participants = new();
        public IReadOnlyCollection<EventParticipant> Participants => _participants.AsReadOnly();

        private readonly List<EventImage> _images = new();
        public IReadOnlyCollection<EventImage> Images => _images.AsReadOnly();

        private Event() { }

        public Event(Guid id, string title, string description, DateTimeOffset date, string venue, Guid categoryId, int capacity)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new InvariantViolationException("Title cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new InvariantViolationException("Description cannot be empty.");
            }
            if (capacity <= 0)
            {
                throw new InvariantViolationException("Capacity must be positive.");
            }

            Id = id;
            Title = title;
            Description = description;
            Date = date;
            Venue = venue;
            CategoryId = categoryId;
            Capacity = capacity;
        }

        public void AddParticipant(Guid participantId)
        {
            if (_participants.Count >= Capacity)
            {
                throw new InvariantViolationException("Event is full.");
            }
            if (_participants.Any(ep => ep.ParticipantId == participantId))
            {
                throw new InvariantViolationException("Participant already registered.");
            }

            _participants.Add(new EventParticipant(Id, participantId));
        }

        public void RemoveParticipant(Guid participantId)
        {
            var existing = _participants.FirstOrDefault(ep => ep.ParticipantId == participantId);
            if (existing == null)
            {
                throw new EntityNotFoundException(participantId);
            }

            _participants.Remove(existing);
        }

        public void UpdateDetails(string title, string description,
                                  DateTimeOffset date, string venue,
                                  Guid categoryId, int capacity)
        {
            if (capacity < _participants.Count)
            {
                throw new InvariantViolationException("The capacity cannot be less than the current number of participants");
            }
            Title = title;
            Description = description;
            Date = date;
            Venue = venue;
            CategoryId = categoryId;
            Capacity = capacity;
        }

        public int ParticipantCount => _participants.Count;

    }
}