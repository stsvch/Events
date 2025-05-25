using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Specifications
{
    public class ParticipantByEmailSpecification : Specification<Participant>
    {
        private readonly string _email;
        public ParticipantByEmailSpecification(string email) => _email = email;
        public override Expression<Func<Participant, bool>> Criteria
            => p => p.Email.Value.Equals(_email, StringComparison.OrdinalIgnoreCase);
    }
}
