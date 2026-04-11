using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts
{
    /// <summary>
    /// واجهة عامة للمستودع مع دعم المفتاح الأساسي من أي نوع
    /// </summary>
    /// <typeparam name="T">نوع الكيان</typeparam>
    /// <typeparam name="TKey">نوع المفتاح الأساسي (int, Guid, long, string...)</typeparam>
    public interface IRepository<T, TKey> where T : class
    {
        // ============== استعلامات أساسية ==============
        Task<T?> GetByIdAsync(TKey id, bool asNoTracking = false, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(TKey id, params Expression<Func<T, object>>[] includes);

        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
                                           bool asNoTracking = true,
                                           CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter,
                                           params Expression<Func<T, object>>[] includes);

        // ============== Pagination ==============
        Task<PagedResult<T>> GetPagedAsync(int pageIndex, int pageSize,
                                           Expression<Func<T, bool>>? filter = null,
                                           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                           bool asNoTracking = true,
                                           CancellationToken cancellationToken = default);

        // ============== Find ==============
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate,
                                         bool asNoTracking = true,
                                         CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate,
                                         params Expression<Func<T, object>>[] includes);

        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
                                      bool asNoTracking = true,
                                      CancellationToken cancellationToken = default);

        // ============== عمليات التعديل ==============
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        // ============== التحقق ==============
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // ============== دعم Specification ==============
        Task<IReadOnlyList<T>> FindWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<int> CountWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T?> GetSingleWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    }
    public interface IRepository<T, TKey> where T : class
    {
        // ... الدوال الأخرى (GetByIdAsync, GetAllAsync, إلخ)

        // دعم Specification
        Task<IReadOnlyList<T>> FindWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<int> CountWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T?> GetSingleWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    }
}