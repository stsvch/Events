using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Exceptions
{
    public class InvariantViolationException : DomainException
    {
        public InvariantViolationException(string message)
            : base(message) { }
    }
}
