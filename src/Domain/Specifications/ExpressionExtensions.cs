using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Specifications
{
    internal static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> Compose<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right,
            Func<Expression, Expression, Expression> merge)
        {
            var map = left.Parameters
                .Select((f, i) => new { f, s = right.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);


            var rightBody = ParameterRebinder.ReplaceParameters(map, right.Body);

            var body = merge(left.Body, rightBody);
            return Expression.Lambda<Func<T, bool>>(body, left.Parameters);
        }
    }
}
