
using System.Linq.Expressions;

namespace Events.Domain.Specifications
{
    public abstract class Specification<T> : ISpecification<T>
    {
        private Func<T, bool>? _predicate;
        public abstract Expression<Func<T, bool>> Criteria { get; }
        public Func<T, bool> Predicate => _predicate ??= Criteria.Compile();
        public ISpecification<T> And(ISpecification<T> other) => new AndSpecification<T>(this, other);
        public ISpecification<T> Or(ISpecification<T> other) => new OrSpecification<T>(this, other);
        public ISpecification<T> Not() => new NotSpecification<T>(this);

        public static Specification<T> True => new TrueSpecification<T>();

        public static Specification<T> False => new FalseSpecification<T>();
    }

}
