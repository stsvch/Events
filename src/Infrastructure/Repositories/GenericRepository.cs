using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using Events.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Events.Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
            => _context = context;

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Set<T>().FindAsync(new object[] { id }, ct);

        public virtual async Task<IEnumerable<T>> ListAsync(ISpecification<T> spec, CancellationToken ct = default)
        {
            var query = SpecificationEvaluator.GetQuery(_context.Set<T>().AsQueryable(), spec);
            return await query.ToListAsync(ct);
        }

        public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _context.Set<T>().AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await GetByIdAsync(id, ct)
                         ?? throw new EntityNotFoundException(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
                entry = _context.Entry(entity);
            }

            entry.State = EntityState.Modified;

            await _context.SaveChangesAsync(ct);
        }


        public async Task<IEnumerable<T>> ListAllAsync(CancellationToken ct = default)
            => await _context.Set<T>().ToListAsync(ct);
    }

}
