using Events.Domain.Common;
using Events.Domain.Entities;
using Events.Domain.Specifications;

namespace Events.Domain.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<PagedList<Event>> ListAsync(
           ISpecification<Event> spec,
           int pageNumber,
           int pageSize,
           bool includeDetails = false,
           CancellationToken ct = default);
        Task<Event?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
        Task<Event?> GetByTitleAsync(
            string title,
            CancellationToken cancellationToken = default);

        Task<Event?> GetByTitleWithDetailsAsync(
            string title,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> SearchByTitleAsync(
            string searchTerm,
            int maxResults = 10,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> SearchByTitleWithDetailsAsync(
            string searchTerm,
            int maxResults = 10,
            CancellationToken cancellationToken = default);
    }
}
