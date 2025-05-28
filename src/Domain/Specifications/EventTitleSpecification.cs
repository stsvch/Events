using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Specifications
{
    public class EventTitleSpecification : Specification<Event>
    {
        private readonly string _term;

        public EventTitleSpecification(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                throw new ArgumentException("Search term cannot be empty.", nameof(term));

            _term = term.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Event, bool>> Criteria
            => e => e.Title.ToLower().Contains(_term);
    }
}
