using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Repositories
{
    public interface IParticipantRepository : IRepository<Participant>
    {
        public Task<Participant?> GetByEmailAsync(string email, CancellationToken ct = default);

    }
}
