using Moeen.Shared.Requests.ImportExport;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.ImportExport;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IImportExportService
    {
        /// <summary>
        /// تصدير سجل الحفظ والمراجعة لطالب بصيغة محددة
        /// </summary>
        Task<ExportStudentRecordResponse> ExportStudentRecordAsync(ExportStudentRecordRequest request);

        /// <summary>
        /// استيراد سجل لطالب من ملف
        /// </summary>
        Task<ImportStudentRecordResponse> ImportStudentRecordAsync(ImportStudentRecordRequest request);

        /// <summary>
        /// [DELETE] إلغاء عملية تصدير/استيراد قيد التنفيذ
        /// </summary>
        Task<OperationResponseDto> CancelJobAsync(CancelJobRequest request);

        /// <summary>
        /// [GET] جلب حالة عملية تصدير/استيراد
        /// </summary>
        Task<ImportExportJobStatusDto> GetJobStatusAsync(GetJobStatusRequest request);
    }
}