using Moeen.Shared.Requests.Backup;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    /// <summary>
    /// خدمة النسخ الاحتياطي والاستعادة (للمالك فقط)
    /// </summary>
    public interface IBackupService
    {
        /// <summary>
        /// إنشاء نسخة احتياطية كاملة للبيانات (ملف .bak)
        /// </summary>
        Task<GeneralResponse> CreateBackupAsync();

        /// <summary>
        /// جلب معلومات آخر نسخة احتياطية (التاريخ، الحجم، الاسم)
        /// </summary>
        Task<GeneralResponse> GetLastBackupInfoAsync();

        /// <summary>
        /// استعادة البيانات من ملف نسخة احتياطي مرفوع
        /// </summary>
        Task<GeneralResponse> RestoreBackupAsync(RestoreBackupRequest request);
    }
}