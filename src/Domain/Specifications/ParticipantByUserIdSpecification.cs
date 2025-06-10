using Events.Domain.Entities;
using System.Linq.Expressions;

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
