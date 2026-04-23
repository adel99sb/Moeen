using Moeen.Api.Shared.Requests.Memorization;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.Memorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IMemorizationService
    {
        /// <summary>
        /// تسجيل حفظ صفحة جديدة لطالب
        /// </summary>
        /// <param name="request">بيانات الصفحة المحفوظة</param>
        /// <returns>عدد النقاط المكتسبة</returns>
        Task<RecordPageMemorizationResponse> RecordNewPageMemorizationAsync(RecordPageMemorizationRequest request);

        /// <summary>
        /// تسجيل حفظ مجموعة صفحات دفعة واحدة
        /// </summary>
        /// <param name="request">معرف الطالب وقائمة الصفحات مع التقديرات</param>
        /// <returns>نتيجة العملية مع تفاصيل كل صفحة</returns>
        Task<BatchResult> RecordNewPagesBatchAsync(RecordPagesBatchRequest request);

        /// <summary>
        /// الحصول على آخر صفحة حفظها الطالب
        /// </summary>
        /// <param name="request">معرف الطالب</param>
        /// <returns>رقم آخر صفحة</returns>
        Task<GetLastMemorizedPageResponse> GetLastMemorizedPageAsync(GetLastMemorizedPageRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل حفظ صفحة معينة لطالب
        /// </summary>
        Task<MemorizationRecordDto> GetMemorizationRecordAsync(GetMemorizationRecordRequest request);

        /// <summary>
        /// [GET] جلب تاريخ الحفظ لطالب مع التصفح والتصفية
        /// </summary>
        Task<PagedList<MemorizationRecordDto>> GetStudentMemorizationHistoryAsync(GetStudentMemorizationHistoryRequest request);

        /// <summary>
        /// [GET] جلب إحصائيات حفظ الطالب
        /// </summary>
        Task<MemorizationStatisticsDto> GetMemorizationStatisticsAsync(GetMemorizationStatisticsRequest request);

        /// <summary>
        /// [GET] جلب تقدم طلاب حلقة معينة في الحفظ
        /// </summary>
        Task<List<StudentMemorizationSummaryDto>> GetCircleMemorizationProgressAsync(GetCircleProgressRequest request);

        /// <summary>
        /// [GET] جلب تقرير تقدم الحفظ للطالب (للرسوم البيانية)
        /// </summary>
        Task<MemorizationProgressReportDto> GetMemorizationProgressReportAsync(GetProgressReportRequest request);

        /// <summary>
        /// [PUT] تحديث تقدير صفحة محفوظة
        /// </summary>
        Task<MemorizationRecordDto> UpdateMemorizationGradeAsync(UpdateMemorizationGradeRequest request);

        /// <summary>
        /// [PUT] إعادة تعيين حالة سجل حفظ
        /// </summary>
        Task<OperationResponseDto> ResetMemorizationRecordAsync(ResetMemorizationRecordRequest request);

        /// <summary>
        /// [DELETE] حذف سجل حفظ
        /// </summary>
        Task<OperationResponseDto> DeleteMemorizationRecordAsync(DeleteMemorizationRecordRequest request);

        /// <summary>
        /// [POST] توليد شهادة حفظ إلكترونية
        /// </summary>
        Task<CertificateDto> GenerateMemorizationCertificateAsync(GenerateCertificateRequest request);
    }
}