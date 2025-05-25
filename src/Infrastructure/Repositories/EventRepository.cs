using Events.Domain.Common;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using Events.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Event?> GetByTitleAsync(string title, CancellationToken cancellationToken = default)
        {
            return await _context.Events
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(e => e.Title.Equals(title, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }

        public async Task<Event?> GetByTitleWithDetailsAsync(string title, CancellationToken cancellationToken = default)
        {
            return await _context.Events
                                 .AsNoTracking()
                                 .Include(e => e.Participants)
                                     .ThenInclude(ep => ep.Participant)
                                 .Include(e => e.Images)
                                 .FirstOrDefaultAsync(
                                     e => e.Title.Equals(title, StringComparison.OrdinalIgnoreCase),
                                     cancellationToken);
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
                        .ThenInclude(ep => ep.Participant)
                    .Include(e => e.Images);
            }

            query = query.OrderBy(e => e.Date);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedList<Event>(items, total);
        }

        public async Task<IEnumerable<Event>> SearchByTitleAsync(
            string searchTerm,
            int maxResults = 10,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Array.Empty<Event>();

            var lower = searchTerm.Trim().ToLower();

            return await _context.Events
                                 .AsNoTracking()
                                 .Where(e => e.Title.ToLower().Contains(lower))
                                 .OrderBy(e => e.Title)
                                 .Take(maxResults)
                                 .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Event>> SearchByTitleWithDetailsAsync(
            string searchTerm,
            int maxResults = 10,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Array.Empty<Event>();

            var lower = searchTerm.Trim().ToLower();

            return await _context.Events
                                 .AsNoTracking()
                                 .Where(e => e.Title.ToLower().Contains(lower))
                                 .Include(e => e.Participants)
                                     .ThenInclude(ep => ep.Participant)
                                 .Include(e => e.Images)
                                 .OrderBy(e => e.Title)
                                 .Take(maxResults)
                                 .ToListAsync(cancellationToken);
        }
    }

}
