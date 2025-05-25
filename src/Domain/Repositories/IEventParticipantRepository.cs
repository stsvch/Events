using Events.Domain.Entities;
using Events.Domain.Specifications;

namespace Events.Domain.Repositories
{
    public interface IEventParticipantRepository
    {
        Task AddAsync(EventParticipant entity, CancellationToken cancellationToken = default);

        Task<EventParticipant?> GetByIdAsync(
            Guid eventId,
            Guid participantId,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            EventParticipant entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid eventId,
            Guid participantId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<EventParticipant>> ListByEventAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<EventParticipant>> ListByParticipantAsync(
            Guid participantId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<EventParticipant>> ListAsync(
            ISpecification<EventParticipant> spec,
            CancellationToken cancellationToken = default);
    }

}
