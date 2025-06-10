
using System.Linq.Expressions;

namespace Events.Domain.Specifications
{
    internal class NotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _inner;

        public NotSpecification(ISpecification<T> inner)
        {
            _inner = inner;
        }

        public override Expression<Func<T, bool>> Criteria
            => Expression.Lambda<Func<T, bool>>(
                Expression.Not(_inner.Criteria.Body),
                _inner.Criteria.Parameters);
    }
}
