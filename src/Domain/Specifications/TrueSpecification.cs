
using System.Linq.Expressions;

namespace Events.Domain.Specifications
{
    internal class TrueSpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> Criteria => x => true;
    }
}
