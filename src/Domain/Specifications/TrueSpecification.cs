using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Specifications
{
    internal class TrueSpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> Criteria => x => true;
    }
}
