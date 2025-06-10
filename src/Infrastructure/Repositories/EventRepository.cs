using Events.Domain.Common;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using Events.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context) { }

        public async Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }


        public async Task<Event?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Participants)              
                    .ThenInclude(ep => ep.Participant)    
                .Include(e => e.Images)                   
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<PagedList<Event>> ListAsync(
            ISpecification<Event> spec,
            int pageNumber,
            int pageSize,
            bool includeDetails = false,
            CancellationToken ct = default)
        {
            var query = SpecificationEvaluator.GetQuery(
                            _context.Events.AsNoTracking(), spec);

            if (includeDetails)
            {
                query = query
                    .Include(e => e.Participants)
                        .ThenInclude(ep => ep.Participant);
            }

            query = query.OrderBy(e => e.Date);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedList<Event>(items, total);
        }
    }

}
