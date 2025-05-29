using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Repositories
{
    public interface IEventImageRepository: IRepository<EventImage>
    {
        Task<IEnumerable<EventImage>> ListByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
