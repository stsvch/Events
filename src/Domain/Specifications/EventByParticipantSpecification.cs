using Events.Domain.Entities;
using System.Linq.Expressions;
namespace Events.Domain.Specifications
{
    public class EventByParticipantSpecification : Specification<Event>
    {
        private readonly Guid _participantId;

        public EventByParticipantSpecification(Guid participantId)
        {
            _participantId = participantId;
        }

        public override Expression<Func<Event, bool>> Criteria
            => evt => evt.Participants.Any(ep => ep.ParticipantId == _participantId);
    }
}
