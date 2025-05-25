using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
            => participant => participant.EventId == _eventId;
    }
}
