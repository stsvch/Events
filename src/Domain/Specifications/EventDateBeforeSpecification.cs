using Events.Domain.Entities;
using System.Linq.Expressions;

namespace Events.Domain.Specifications
{
    public class EventDateBeforeSpecification : Specification<Event>
    {
        private readonly DateTimeOffset _end;
        public EventDateBeforeSpecification(DateTimeOffset end) => _end = end;
        public override Expression<Func<Event, bool>> Criteria
            => evt => evt.Date <= _end;
    }
}
