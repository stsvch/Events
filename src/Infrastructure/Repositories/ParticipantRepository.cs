using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using Events.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Repositories
{
    public class ParticipantRepository : GenericRepository<Participant>, IParticipantRepository
    {
        public ParticipantRepository(AppDbContext ctx) : base(ctx) { }

        public async Task<bool> IsUserRegisteredAsync(Guid eventId, string userId, CancellationToken ct = default)
        {
            var participant = await _context.Participants
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

            if (participant == null)
                return false;

            return await _context.Participants
                                 .Where(p => p.Id == participant.Id)
                                 .SelectMany(p => p.Participations)
                                 .AnyAsync(ep => ep.EventId == eventId, ct);
        }

        public async Task<Participant?> GetBySpecAsync(ISpecification<Participant> spec, CancellationToken ct = default)
        {
            var query = SpecificationEvaluator.GetQuery(_context.Participants.AsNoTracking(), spec);
            return await query.FirstOrDefaultAsync(ct);
        }

    }
}
