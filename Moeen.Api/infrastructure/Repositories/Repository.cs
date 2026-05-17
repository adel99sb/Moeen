using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.infrastructure.Data;

namespace Moeen.Api.infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(ISpecification<T> specification = null)
        {
            IQueryable<T> query = _dbSet;

            if (specification != null)
            {
                if (specification.Criteria != null)
                    query = query.Where(specification.Criteria);

                // ✅ legacy includes
                foreach (var inc in specification.Includes)
                    query = query.Include(inc);

                // ✅ new include chains
                foreach (var chain in specification.IncludeChains)
                    query = chain(query);

                if (specification.OrderBy != null)
                    query = query.OrderBy(specification.OrderBy);

                if (specification.OrderByDescending != null)
                    query = query.OrderByDescending(specification.OrderByDescending);

                if (specification.Skip.HasValue)
                    query = query.Skip(specification.Skip.Value);

                if (specification.Take.HasValue)
                    query = query.Take(specification.Take.Value);
            }

            return await query.ToListAsync();
        }

        public IQueryable<T> GetAllQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}