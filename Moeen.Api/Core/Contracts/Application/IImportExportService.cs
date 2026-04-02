using Moeen.Api.Shared.Requests.ImportExport;
using Moeen.Api.Shared.Responses.ImportExport;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IImportExportService
    {
        /// <summary>
        /// تصدير سجل الحفظ والمراجعة لطالب بصيغة محددة
        /// </summary>
        /// <param name="request">معرف الطالب والصيغة المطلوبة</param>
        /// <returns>ملف السجل مع معلوماته</returns>
        Task<ExportStudentRecordResponse> ExportStudentRecordAsync(ExportStudentRecordRequest request);

        /// <summary>
        /// استيراد سجل لطالب من ملف (إذا كان الطالب منقولاً من مركز آخر)
        /// </summary>
        /// <param name="request">معرف الطالب وبيانات الملف</param>
        /// <returns>نتيجة الاستيراد</returns>
        Task<ImportStudentRecordResponse> ImportStudentRecordAsync(ImportStudentRecordRequest request);
    }
}