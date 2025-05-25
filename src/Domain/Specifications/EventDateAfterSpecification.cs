using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Specifications
{
    public class EventDateAfterSpecification : Specification<Event>
    {
        private readonly DateTimeOffset _start;
        public EventDateAfterSpecification(DateTimeOffset start) => _start = start;
        public override Expression<Func<Event, bool>> Criteria
            => evt => evt.Date >= _start;
    }
}
