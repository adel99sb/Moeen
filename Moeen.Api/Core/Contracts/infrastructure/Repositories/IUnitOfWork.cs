using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        // المستودع العام (يدعم المفتاح العام TKey)
        IRepository<T, TKey> Repository<T, TKey>() where T : class;

        // المستودع المخصص (الذي يتم تسجيله في DI)
        TRepository CustomRepository<TRepository>() where TRepository : class;

        // حفظ التغييرات (مع دعم CancellationToken)
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // المعاملات الصريحة
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        // مسح التتبع (لتحسين الأداء)
        void ClearTracking();

        // حفظ مجزأ للمجموعات الكبيرة (اختياري)
        Task<int> SaveChangesInBatchesAsync(
            IEnumerable<object> entities,
            int batchSize = 100,
            CancellationToken cancellationToken = default);
    }
}