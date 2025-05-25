using Events.Domain.Common;
using Events.Domain.Entities;
using Events.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<Event?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);

        Task<PagedList<Event>> ListAsync(
    ISpecification<Event> spec,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default);
    }
}
