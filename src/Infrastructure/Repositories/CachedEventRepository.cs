using Events.Application.Interfaces;
using Events.Domain.Common;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Repositories
{
    public class CachedEventRepository : IEventRepository
    {
        private const string ByIdKey = "event:id:{0}";
        private const string ListKey = "event:list:{0}:{1}:{2}"; // spec hash, page, size
        private readonly IEventRepository _inner;
        private readonly ICacheService _cache;
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

        public CachedEventRepository(
            IEventRepository inner,
            ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var key = string.Format(ByIdKey, id);
            var ev = await _cache.GetAsync<Event>(key, ct);
            if (ev != null) return ev;

            ev = await _inner.GetByIdAsync(id, ct);
            if (ev != null)
                await _cache.SetAsync(key, ev, _ttl, ct);

            return ev;
        }

        public async Task<Event?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
        {
            // Можно отдельно кешировать детали, или просто пропускать через inner
            return await _inner.GetByIdWithDetailsAsync(id, ct);
        }

        public async Task<Event?> GetByTitleAsync(string title, CancellationToken ct = default)
            => await _inner.GetByTitleAsync(title, ct);

        public async Task<Event?> GetByTitleWithDetailsAsync(string title, CancellationToken ct = default)
            => await _inner.GetByTitleWithDetailsAsync(title, ct);

        public async Task<IEnumerable<Event>> SearchByTitleAsync(
            string searchTerm, int maxResults = 10, CancellationToken ct = default)
            => await _inner.SearchByTitleAsync(searchTerm, maxResults, ct);

        public async Task<IEnumerable<Event>> SearchByTitleWithDetailsAsync(
            string searchTerm, int maxResults = 10, CancellationToken ct = default)
            => await _inner.SearchByTitleWithDetailsAsync(searchTerm, maxResults, ct);

        public async Task<PagedList<Event>> ListAsync(
            ISpecification<Event> spec,
            int pageNumber,
            int pageSize,
            bool includeDetails = false,
            CancellationToken ct = default)
        {
            // Генерируем ключ по хэшу спецификации + страницу + размер
            var specHash = spec.Criteria.GetHashCode();
            var key = string.Format(ListKey, specHash, pageNumber, pageSize);

            var cached = await _cache.GetAsync<PagedList<Event>>(key, ct);
            if (cached != null) return cached;

            var list = await _inner.ListAsync(spec, pageNumber, pageSize, includeDetails, ct);
            await _cache.SetAsync(key, list, _ttl, ct);
            return list;
        }

        // GenericRepository methods (Add, Update, Delete) — проксируем и очищаем кеш
        public async Task AddAsync(Event entity, CancellationToken ct = default)
        {
            await _inner.AddAsync(entity, ct);
            await _cache.RemoveAsync(string.Format(ByIdKey, entity.Id), ct);
        }

        public async Task UpdateAsync(Event entity, CancellationToken ct = default)
        {
            await _inner.UpdateAsync(entity, ct);
            await _cache.RemoveAsync(string.Format(ByIdKey, entity.Id), ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            await _inner.DeleteAsync(id, ct);
            await _cache.RemoveAsync(string.Format(ByIdKey, id), ct);
        }

        public Task<IEnumerable<Event>> ListAsync(ISpecification<Event> specification, CancellationToken cancellationToken = default)
        {
            return _inner.ListAsync(specification, cancellationToken);
        }
    }
}
