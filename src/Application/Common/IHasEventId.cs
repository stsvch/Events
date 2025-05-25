using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Common
{
    public interface IHasEventId
    {
        Guid EventId { get; }
    }
}
