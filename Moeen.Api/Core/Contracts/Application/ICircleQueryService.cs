using Moeen.Api.Shared.Requests.CircleQuery;
using Moeen.Api.Shared.Responses.Circle;
using Moeen.Api.Shared.Responses.CircleQuery;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ICircleQueryService
    {
        /// <summary>
        /// الحصول على تفاصيل حلقة محددة
        /// </summary>
        Task<CircleDto> GetCircleByIdAsync(GetCircleByIdRequest request);

        /// <summary>
        /// الحصول على قائمة الطلاب المسجلين في الحلقة مع إمكانية التصفية والتصفح
        /// </summary>
        Task<CircleStudentsResponse> GetCircleStudentsAsync(GetCircleStudentsRequest request);

        /// <summary>
        /// الحصول على عدد الطلاب المسجلين في الحلقة
        /// </summary>
        Task<CircleStudentsCountResponse> GetCircleStudentsCountAsync(GetCircleStudentsCountRequest request);

        /// <summary>
        /// [GET] جلب إحصائيات وتقدم الحلقة (متوسط الحفظ، نسبة الحضور، التقييم)
        /// </summary>
        Task<CircleStatisticsDto> GetCircleStatisticsAsync(GetCircleStatisticsRequest request);

        /// <summary>
        /// [GET] جلب جدول الحضور والغياب للحلقة خلال فترة زمنية محددة
        /// </summary>
        Task<CircleAttendanceReportResponse> GetCircleAttendanceReportAsync(GetCircleAttendanceReportRequest request);
    }
}