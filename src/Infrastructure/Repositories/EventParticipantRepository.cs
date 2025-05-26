using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using Events.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Repositories
{
    public class EventParticipantRepository : IEventParticipantRepository
    {
        private readonly AppDbContext _context;

        public EventParticipantRepository(AppDbContext context)
            => _context = context;

        public async Task AddAsync(
            EventParticipant entity,
            CancellationToken cancellationToken = default)
        {
            _context.Set<EventParticipant>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<EventParticipant?> GetByIdAsync(
            Guid eventId,
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<EventParticipant>()
                .FindAsync(new object[] { eventId, participantId }, cancellationToken);
        }

        public async Task UpdateAsync(
            EventParticipant entity,
            CancellationToken cancellationToken = default)
        {
            _context.Set<EventParticipant>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            Guid eventId,
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<EventParticipant>()
                .FindAsync(new object[] { eventId, participantId }, cancellationToken);
            if (entity == null)
                throw new EntityNotFoundException(eventId);

            _context.Set<EventParticipant>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<EventParticipant>> ListByEventAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<EventParticipant>()
                .Include(ep => ep.Participant)
                .Where(ep => ep.EventId == eventId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<EventParticipant>> ListByParticipantAsync(
            Guid participantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<EventParticipant>()
                .Include(ep => ep.Event)
                .Where(ep => ep.ParticipantId == participantId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<EventParticipant>> ListAsync(
            ISpecification<EventParticipant> spec,
            CancellationToken cancellationToken = default)
        {
            var query = SpecificationEvaluator.GetQuery(
                _context.Set<EventParticipant>().AsQueryable(), spec);

            return await query.ToListAsync(cancellationToken);
        }
    }

}
