using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
                                 .AsNoTracking()
                                 .Where(img => img.EventId == eventId)
                                 .ToListAsync(cancellationToken);
        }

    }
}
