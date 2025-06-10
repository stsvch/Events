using Events.Domain.Entities;
using System.Linq.Expressions;

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
