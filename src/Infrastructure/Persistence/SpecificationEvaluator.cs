using Events.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
