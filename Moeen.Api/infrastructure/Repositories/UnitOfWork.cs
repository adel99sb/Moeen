
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Moeen.Api.Core.Contracts;

namespace Moeen.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;                     // استخدم DbContext بدلاً من AppDbContext
        private readonly IServiceProvider _serviceProvider;
        private IDbContextTransaction? _currentTransaction;
        private readonly Dictionary<Type, object> _repositories;      // للمستودعات العامة
        private readonly Dictionary<Type, object> _customRepositories; // للمستودعات المخصصة
        private bool _disposed;

        public UnitOfWork(DbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _repositories = new Dictionary<Type, object>();
            _customRepositories = new Dictionary<Type, object>();
        }

        // ============= المستودع العام (مع TKey) =============
        public virtual IRepository<T, TKey> Repository<T, TKey>() where T : class
        {
            var entityType = typeof(T);
            if (!_repositories.ContainsKey(entityType))
            {
                var repositoryInstance = new Repository<T, TKey>(_dbContext);
                _repositories[entityType] = repositoryInstance;
            }
            return (IRepository<T, TKey>)_repositories[entityType];
        }

        // ============= المستودع المخصص =============
        public virtual TRepository CustomRepository<TRepository>() where TRepository : class
        {
            var type = typeof(TRepository);
            if (!_customRepositories.ContainsKey(type))
            {
                // إنشاء المستودع المخصص باستخدام ServiceProvider وتمرير DbContext
                var repository = ActivatorUtilities.CreateInstance<TRepository>(_serviceProvider, _dbContext);
                _customRepositories[type] = repository;
            }
            return (TRepository)_customRepositories[type];
        }

        // ============= حفظ التغييرات =============
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // ============= المعاملات الصريحة =============
        public virtual async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No transaction in progress.");

            try
            {
                // لا نستدعي SaveChangesAsync هنا - المستخدم مسؤول عن استدعائه قبل Commit
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No transaction in progress.");

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ (يمكن إضافة ILogger)
                Console.WriteLine($"Rollback failed: {ex.Message}");
                throw;  // أو معالجة حسب الحاجة
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        // ============= مسح التتبع =============
        public virtual void ClearTracking()
        {
            _dbContext.ChangeTracker.Clear();
        }

        // ============= حفظ مجزأ (للمجموعات الكبيرة) =============
        public virtual async Task<int> SaveChangesInBatchesAsync(IEnumerable<object> entities, int batchSize = 100, CancellationToken cancellationToken = default)
        {
            int totalSaved = 0;
            var batch = new List<object>();
            foreach (var entity in entities)
            {
                batch.Add(entity);
                if (batch.Count >= batchSize)
                {
                    totalSaved += await _dbContext.SaveChangesAsync(cancellationToken);
                    _dbContext.ChangeTracker.Clear();
                    batch.Clear();
                }
            }
            if (batch.Any())
                totalSaved += await _dbContext.SaveChangesAsync(cancellationToken);
            return totalSaved;
        }

        // ============= التخلص من الموارد =============
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _currentTransaction?.Dispose();
                // لا نتخلص من DbContext هنا - الحاوية تدارها
                // _dbContext.Dispose(); // تم إزالتها بناءً على التوصية
            }
            _disposed = true;
        }
    }
}