using Events.Domain.Entities;
using System.Linq.Expressions;

namespace Events.Domain.Specifications
{
    public class EventVenueSpecification : Specification<Event>
    {
        private readonly string _venue;
        public EventVenueSpecification(string venue) => _venue = venue;
        public override Expression<Func<Event, bool>> Criteria
            => evt => evt.Venue.Contains(_venue);
    }
}
