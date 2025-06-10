
using System.Linq.Expressions;

namespace Events.Domain.Specifications
{
    internal class OrSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> Criteria
            => _left.Criteria.Compose(_right.Criteria, Expression.OrElse);
    }
}
