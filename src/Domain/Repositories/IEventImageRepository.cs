using Events.Domain.Entities;

namespace Events.Domain.Repositories
{
    public interface IEventImageRepository: IRepository<EventImage>
    {
        Task<IEnumerable<EventImage>> ListByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
