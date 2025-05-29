using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Repositories
{
    class EventImageRepository: GenericRepository<EventImage>, IEventImageRepository
    {
        public EventImageRepository(AppDbContext context): base(context) { }

        public async Task<IEnumerable<EventImage>> ListByEventIdAsync(
    Guid eventId,
    CancellationToken cancellationToken = default)
        {
            return await _context.Set<EventImage>()
                                 .Where(img => img.EventId == eventId)
                                 .ToListAsync(cancellationToken);
        }

    }
}
