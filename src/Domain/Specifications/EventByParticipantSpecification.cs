using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
            => evt => evt.Participants.Any(p => p.Id == _participantId);
    }
}
