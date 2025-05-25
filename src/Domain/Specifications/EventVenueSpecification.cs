using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
