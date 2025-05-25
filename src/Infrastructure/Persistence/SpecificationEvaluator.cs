using Events.Domain.Specifications;

namespace Events.Infrastructure.Persistence
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(
            IQueryable<T> inputQuery,
            ISpecification<T>? spec)
            where T : class
        {
            if (spec?.Criteria != null)
                inputQuery = inputQuery.Where(spec.Criteria);

            return inputQuery;
        }
    }
}
