using Events.Domain.Entities;
using System.Linq.Expressions;

namespace Events.Domain.Specifications
{
    public class ParticipantByEventSpecification : Specification<Participant>
    {
        private readonly Guid _eventId;
        public ParticipantByEventSpecification(Guid eventId)
        {
            _eventId = eventId;
        }
        public override Expression<Func<Participant, bool>> Criteria
            => p => p.Participations.Any(ep => ep.EventId == _eventId);
    }
}
