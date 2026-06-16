using Moeen.Shared.Requests.HalqaQuery;
using Moeen.Shared.Responses.Halqa;
using Moeen.Shared.Responses.HalqaQuery;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IHalqaQueryService
    {
        /// <summary>
        /// الحصول على تفاصيل حلقة محددة
        /// </summary>
        Task<HalqaDto> GetHalqaByIdAsync(GetHalqaByIdRequest request);

        /// <summary>
        /// جلب كل الحلقات مع إمكانية التصفية حسب المسجد.
        /// </summary>
        Task<List<HalqaDto>> GetAllHalqasAsync(Guid? mosqueId = null);

        /// <summary>
        /// الحصول على قائمة الطلاب المسجلين في الحلقة مع إمكانية التصفية والتصفح
        /// </summary>
        Task<HalqaStudentsResponse> GetHalqaStudentsAsync(GetHalqaStudentsRequest request);

        /// <summary>
        /// الحصول على عدد الطلاب المسجلين في الحلقة
        /// </summary>
        Task<HalqaStudentsCountResponse> GetHalqaStudentsCountAsync(GetHalqaStudentsCountRequest request);

        /// <summary>
        /// [GET] جلب إحصائيات وتقدم الحلقة (متوسط الحفظ، نسبة الحضور، التقييم)
        /// </summary>
        Task<HalqaStatisticsDto> GetHalqaStatisticsAsync(GetHalqaStatisticsRequest request);

        /// <summary>
        /// [GET] جلب جدول الحضور والغياب للحلقة خلال فترة زمنية محددة
        /// </summary>
        Task<HalqaAttendanceReportResponse> GetHalqaAttendanceReportAsync(GetHalqaAttendanceReportRequest request);
    }
}