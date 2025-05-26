using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Specifications
{
    public class ParticipantByIdSpecification : Specification<Participant>
    {
        private readonly Guid _id;

        public ParticipantByIdSpecification(Guid id)
        {
            _id = id;
        }

        public override Expression<Func<Participant, bool>> Criteria
            => p => p.Id == _id;
    }
}
