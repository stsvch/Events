using Events.Domain.Entities;
using Events.Domain.Exceptions;
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
    public class EventParticipantRepository
            : IEventParticipantRepository
    {
        private readonly AppDbContext _context;

        public EventParticipantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            EventParticipant entity,
            CancellationToken cancellationToken = default)
        {
            _context.Set<EventParticipant>().Add(entity);
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

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            // Не используется для составных ключей. Вызывать DeleteAsync(eventId, participantId).
            throw new NotSupportedException("Use DeleteAsync(eventId, participantId) instead.");
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


        public async Task<IEnumerable<EventParticipant>> ListAsync(ISpecification<EventParticipant> spec, CancellationToken ct = default)
        {
            var query = SpecificationEvaluator.GetQuery(_context.Set<EventParticipant>().AsQueryable(), spec);
            return await query.ToListAsync(ct);
        }

        public async Task UpdateAsync(
            EventParticipant entity,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<EventParticipant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
