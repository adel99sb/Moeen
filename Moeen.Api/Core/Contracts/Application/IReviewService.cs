using Moeen.Api.Shared.Requests.Review;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.Review;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IReviewService
    {
        /// <summary>
        /// تسجيل مراجعة صفحة لطالب
        /// </summary>
        Task<RecordReviewPageResponse> RecordReviewPageAsync(RecordReviewPageRequest request);

        /// <summary>
        /// تسجيل مراجعة جزء كامل
        /// </summary>
        Task<ReviewResultDto> RecordJuzReviewAsync(RecordJuzReviewRequest request);

        /// <summary>
        /// تسجيل مراجعة عدة أجزاء دفعة واحدة
        /// </summary>
        Task<MultipleJuzReviewResponse> RecordMultipleJuzReviewAsync(RecordMultipleJuzReviewRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل مراجعة معينة
        /// </summary>
        Task<ReviewRecordDto> GetReviewRecordByIdAsync(GetReviewRecordByIdRequest request);

        /// <summary>
        /// [GET] جلب سجل مراجعات الطالب
        /// </summary>
        Task<PagedList<ReviewRecordDto>> GetStudentReviewHistoryAsync(GetStudentReviewHistoryRequest request);

        /// <summary>
        /// [GET] جلب مراجعات طالب ضمن فترة زمنية
        /// </summary>
        Task<List<ReviewRecordDto>> GetReviewsByDateRangeAsync(GetReviewsByDateRangeRequest request);

        /// <summary>
        /// [GET] جلب تقدم مراجعات طلاب حلقة
        /// </summary>
        Task<List<StudentReviewSummaryDto>> GetCircleReviewProgressAsync(GetCircleReviewProgressRequest request);

        /// <summary>
        /// [GET] جلب مراجعات حسب المعلم
        /// </summary>
        Task<List<TeacherReviewSummaryDto>> GetReviewsByTeacherAsync(GetReviewsByTeacherRequest request);

        /// <summary>
        /// [PUT] تحديث تقييم مراجعة
        /// </summary>
        Task<ReviewRecordDto> UpdateReviewGradeAsync(UpdateReviewGradeRequest request);

        /// <summary>
        /// [DELETE] حذف سجل مراجعة
        /// </summary>
        Task<ReviewOperationResponse> DeleteReviewRecordAsync(DeleteReviewRecordRequest request);

        /// <summary>
        /// [POST] توليد شهادة مراجعة إلكترونية
        /// </summary>
        Task<ReviewCertificateDto> GenerateReviewCertificateAsync(GenerateReviewCertificateRequest request);
    }
}