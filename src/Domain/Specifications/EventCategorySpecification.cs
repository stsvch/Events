using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
