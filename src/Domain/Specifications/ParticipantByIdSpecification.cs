using Events.Domain.Entities;
using System.Linq.Expressions;

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
