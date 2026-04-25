using Moeen.Api.Shared.Requests.Points;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.Points;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IPointsService
    {
        /// <summary>
        /// إعداد نظام النقاط بتعيين عدد النقاط لكل تقدير
        /// </summary>
        Task<SetupPointsSystemResponse> SetupPointsSystemAsync(SetupPointsSystemRequest request);

        /// <summary>
        /// الحصول على رصيد نقاط الطالب الحالي
        /// </summary>
        Task<GetStudentPointsResponse> GetStudentPointsAsync(GetStudentPointsRequest request);

        /// <summary>
        /// الحصول على سجل النقاط المفصل للطالب
        /// </summary>
        Task<GetPointsHistoryResponse> GetPointsHistoryAsync(GetPointsHistoryRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل معاملة نقاط محددة بمعرفها
        /// </summary>
        Task<PointsTransactionDto> GetTransactionByIdAsync(GetTransactionByIdRequest request);

        /// <summary>
        /// [GET] جلب قواعد نظام النقاط الحالية
        /// </summary>
        Task<List<PointRuleDto>> GetPointRulesAsync(GetPointRulesRequest request);

        /// <summary>
        /// [GET] جلب لوحة الصدارة
        /// </summary>
        Task<PagedList<StudentLeaderboardDto>> GetPointsLeaderboardAsync(GetLeaderboardRequest request);

        /// <summary>
        /// [GET] جلب أنواع النقاط المتاحة
        /// </summary>
        Task<List<PointTypeDto>> GetPointTypesAsync(GetPointTypesRequest request);

        /// <summary>
        /// [GET] جلب نقاط طلاب حلقة معينة
        /// </summary>
        Task<List<StudentPointsSummaryDto>> GetCirclePointsSummaryAsync(GetCirclePointsSummaryRequest request);

        /// <summary>
        /// [PUT] تحديث قاعدة نقاط معينة
        /// </summary>
        Task<PointRuleDto> UpdatePointRuleAsync(UpdatePointRuleRequest request);

        /// <summary>
        /// [DELETE] حذف قاعدة نقاط معينة
        /// </summary>
        Task<OperationResponseDto> DeletePointRuleAsync(DeletePointRuleRequest request);

        /// <summary>
        /// [POST] منح نقاط يدوياً لطالب
        /// </summary>
        Task<PointsTransactionDto> AwardPointsManuallyAsync(AwardPointsManualRequest request);
    }
}