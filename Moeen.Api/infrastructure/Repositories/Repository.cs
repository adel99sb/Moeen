using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts;

namespace Moeen.Infrastructure.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ============== الاستعلامات الأساسية ==============
        public virtual async Task<T?> GetByIdAsync(TKey id, bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id").Equals(id), cancellationToken);
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<T?> GetByIdAsync(TKey id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);
            return await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id").Equals(id));
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
                bool asNoTracking = true,
             CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
                query = query.Where(filter);
            if (asNoTracking)
                query = query.AsNoTracking();
            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter,
         params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
                query = query.Where(filter);
            foreach (var include in includes)
                query = query.Include(include);
            return await query.ToListAsync();
        }

        // ============== Pagination ==============
        public virtual async Task<PagedResult<T>> GetPagedAsync(int pageIndex, int pageSize,
                Expression<Func<T, bool>>? filter = null,
                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                  bool asNoTracking = true,
                                                                CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy != null)
                query = orderBy(query);

            if (asNoTracking)
                query = query.AsNoTracking();

            var items = await query.Skip(pageIndex * pageSize)
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

        // ============== Find ==============
        public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate,
                                                              bool asNoTracking = true,
                                                              CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            if (asNoTracking)
                query = query.AsNoTracking();
            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate,
                                                              params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            foreach (var include in includes)
                query = query.Include(include);
            return await query.ToListAsync();
        }

        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
                                                           bool asNoTracking = true,
                                                           CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            if (asNoTracking)
                query = query.AsNoTracking();
            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        // ============== عمليات التعديل ==============
        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual void Update(T entity)
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

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        // ============== التحقق ==============
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }

        // ============== دعم Specification ==============
        public virtual async Task<IReadOnlyList<T>> FindWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).ToListAsync(cancellationToken);
        }

        public virtual async Task<int> CountWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).CountAsync(cancellationToken);
        }

        public virtual async Task<T?> GetSingleWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
        }

        // دالة مساعدة لتطبيق المواصفة على IQueryable
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            IQueryable<T> query = _dbSet;

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            foreach (var include in spec.Includes)
                query = query.Include(include);

            foreach (var includeString in spec.IncludeStrings)
                query = query.Include(includeString);

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }
    }
    public class Repository<T, TKey> : IRepository<T, TKey>
    {
        private readonly SpecificationEvaluator<T> _evaluator = new();

        public async Task<IReadOnlyList<T>> FindWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            var query = _evaluator.GetQuery(_dbSet, spec);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<int> CountWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await _evaluator.CountAsync(_dbSet, spec, cancellationToken);
        }

        public async Task<T?> GetSingleWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            var query = _evaluator.GetQuery(_dbSet, spec);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}