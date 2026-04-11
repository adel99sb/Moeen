using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts;
using Moeen.Api.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Moeen.Api.infrastructure.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;
        private readonly SpecificationEvaluator<T> _specEvaluator = new();

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(TKey id, bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
            {
                return await _dbSet
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), cancellationToken);
            }

            return await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
        }

        public async Task<T?> GetByIdAsync(TKey id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id));
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (filter is not null)
                query = query.Where(filter);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (filter is not null)
                query = query.Where(filter);

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<T>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (filter is not null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy is not null)
                query = orderBy(query);

            if (asNoTracking)
                query = query.AsNoTracking();

            var items = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<IReadOnlyList<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<T?> SingleOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        public Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => _dbSet.AddAsync(entity, cancellationToken).AsTask();

        public void Update(T entity)
        {
            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
            else
            {
                _dbSet.Update(entity);
            }
        }

        public void Delete(T entity) => _dbSet.Remove(entity);
        public void DeleteRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => _dbSet.AnyAsync(predicate, cancellationToken);

        public Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => _dbSet.CountAsync(predicate, cancellationToken);

        public async Task<IReadOnlyList<T>> FindWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            var query = _specEvaluator.GetQuery(_dbSet.AsQueryable(), spec);
            return await query.ToListAsync(cancellationToken);
        }

        public Task<int> CountWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
            => _specEvaluator.CountAsync(_dbSet.AsQueryable(), spec, cancellationToken);

        public async Task<T?> GetSingleWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            var query = _specEvaluator.GetQuery(_dbSet.AsQueryable(), spec);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}