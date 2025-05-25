using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Repositories
{
    public interface IEventParticipantRepository : IRepository<EventParticipant>
    {
        Task<IEnumerable<EventParticipant>> ListByEventAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<EventParticipant>> ListByParticipantAsync(
            Guid participantId,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid eventId, Guid participantId, CancellationToken cancellationToken = default);
    }
}
