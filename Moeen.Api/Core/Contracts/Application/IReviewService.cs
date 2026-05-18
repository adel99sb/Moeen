using Moeen.Shared.Requests.Review;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IReviewService
    {
        /// <summary>
        /// تسجيل مراجعة صفحة (مراجعة يومية)
        /// </summary>
        Task<GeneralResponse> RecordReviewPageAsync(RecordReviewPageRequest request);

        /// <summary>
        /// تسجيل مراجعة جزء كامل
        /// </summary>
        Task<GeneralResponse> RecordJuzReviewAsync(RecordJuzReviewRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل مراجعة معينة
        /// </summary>
        Task<GeneralResponse> GetReviewRecordByIdAsync(GetReviewRecordByIdRequest request);

        /// <summary>
        /// [GET] جلب سجل مراجعات الطالب
        /// </summary>
        Task<GeneralResponse> GetStudentReviewHistoryAsync(GetStudentReviewHistoryRequest request);

        /// <summary>
        /// [PUT] تحديث تقييم مراجعة
        /// </summary>
        Task<GeneralResponse> UpdateReviewGradeAsync(UpdateReviewGradeRequest request);

        /// <summary>
        /// [DELETE] حذف سجل مراجعة
        /// </summary>
        Task<GeneralResponse> DeleteReviewRecordAsync(DeleteReviewRecordRequest request);
    }
}