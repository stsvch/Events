
using System.Linq.Expressions;

namespace Events.Domain.Specifications
{
    internal class FalseSpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> Criteria => x => false;
    }
}
