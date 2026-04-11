using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Moeen.Api.Core.Contracts;
using Moeen.Api.infrastructure.Data;    
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Moeen.Api.infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;

        private IDbContextTransaction? _currentTransaction;
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly Dictionary<Type, object> _customRepositories = new();
        private bool _disposed;

        public UnitOfWork(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IRepository<T, TKey> Repository<T, TKey>() where T : class
        {
            var entityType = typeof(T);

            if (!_repositories.TryGetValue(entityType, out var repository))
            {
                repository = new Repository<T, TKey>(_dbContext);
                _repositories[entityType] = repository;
            }

            return (IRepository<T, TKey>)repository;
        }

        public TRepository CustomRepository<TRepository>() where TRepository : class
        {
            var type = typeof(TRepository);

            if (!_customRepositories.TryGetValue(type, out var repository))
            {
                repository = ActivatorUtilities.CreateInstance<TRepository>(_serviceProvider, _dbContext);
                _customRepositories[type] = repository;
            }

            return (TRepository)repository;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dbContext.SaveChangesAsync(cancellationToken);

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is not null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null)
                throw new InvalidOperationException("No transaction in progress.");

            try
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null)
                throw new InvalidOperationException("No transaction in progress.");

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public void ClearTracking() => _dbContext.ChangeTracker.Clear();

        public async Task<int> SaveChangesInBatchesAsync(IEnumerable<object> entities, int batchSize = 100, CancellationToken cancellationToken = default)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            if (batchSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(batchSize));

            var totalSaved = 0;
            var batch = new List<object>(batchSize);

            foreach (var entity in entities)
            {
                batch.Add(entity);

                if (batch.Count < batchSize)
                    continue;

                await _dbContext.AddRangeAsync(batch, cancellationToken);
                totalSaved += await _dbContext.SaveChangesAsync(cancellationToken);
                _dbContext.ChangeTracker.Clear();
                batch.Clear();
            }

            if (batch.Any())
            {
                await _dbContext.AddRangeAsync(batch, cancellationToken);
                totalSaved += await _dbContext.SaveChangesAsync(cancellationToken);
                _dbContext.ChangeTracker.Clear();
            }

            return totalSaved;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _currentTransaction?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}