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
    public class ParticipantRepository : GenericRepository<Participant>, IParticipantRepository
    {
        public ParticipantRepository(AppDbContext ctx) : base(ctx) { }

        public async Task<Participant?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Participants
                                 .FirstOrDefaultAsync(p => p.Email.Value == email, ct);
        }

    }
}
