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
    }
}
