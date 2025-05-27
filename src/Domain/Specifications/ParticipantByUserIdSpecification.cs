using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Specifications
{
    public class ParticipantByUserIdSpecification : Specification<Participant>
    {
        private readonly string _userId;
        public ParticipantByUserIdSpecification(string userId) => _userId = userId;
        public override Expression<Func<Participant, bool>> Criteria
            => p => p.UserId == _userId;
    }
}
