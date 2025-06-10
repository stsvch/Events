using Events.Domain.Entities;
using System.Linq.Expressions;


namespace Events.Domain.Specifications
{
    public class EventCategorySpecification : Specification<Event>
    {
        private readonly Guid _categoryId;
        public EventCategorySpecification(Guid categoryId) => _categoryId = categoryId;
        public override Expression<Func<Event, bool>> Criteria
            => evt => evt.CategoryId == _categoryId;
    }
}
