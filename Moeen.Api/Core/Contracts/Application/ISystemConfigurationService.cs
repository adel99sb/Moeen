using Moeen.Api.Shared.Requests.SystemConfiguration;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.SystemConfiguration;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ISystemConfigurationService
    {
        /// <summary>
        /// إدارة إعدادات النظام
        /// </summary>
        Task<ManageSystemSettingsResponse> ManageSystemSettingsAsync(ManageSystemSettingsRequest request);

        /// <summary>
        /// إدارة الفترات الزمنية
        /// </summary>
        Task<ManageTimePeriodsResponse> ManageTimePeriodsAsync(ManageTimePeriodsRequest request);

        /// <summary>
        /// إعدادات التواقيت والمواعيد
        /// </summary>
        Task<ConfigureTimingsResponse> ConfigureTimingsAsync(ConfigureTimingsRequest request);

        /// <summary>
        /// الحصول على سجلات النظام
        /// </summary>
        Task<GetSystemLogsResponse> GetSystemLogsAsync(GetSystemLogsRequest request);

        /// <summary>
        /// مراقبة أداء النظام
        /// </summary>
        Task<MonitorSystemHealthResponse> MonitorSystemHealthAsync(MonitorSystemHealthRequest request);

        /// <summary>
        /// إصدار تراخيص المستخدمين
        /// </summary>
        Task<IssueUserLicenseResponse> IssueUserLicensesAsync(IssueUserLicenseRequest request);

        /// <summary>
        /// [GET] جلب إعدادات النظام الحالية
        /// </summary>
        Task<SystemSettingsDto> GetSystemSettingsAsync(GetSystemSettingsRequest request);

        /// <summary>
        /// [GET] جلب الفترات الزمنية النشطة/الكل مع التصفح
        /// </summary>
        Task<PagedList<TimePeriodDto>> GetAllTimePeriodsAsync(GetAllTimePeriodsRequest request);

        /// <summary>
        /// [GET] جلب فترة زمنية محددة
        /// </summary>
        Task<TimePeriodDto> GetTimePeriodByIdAsync(GetTimePeriodByIdRequest request);

        /// <summary>
        /// [GET] جلب إعدادات الأمان
        /// </summary>
        Task<SecuritySettingsDto> GetSecuritySettingsAsync(GetSecuritySettingsRequest request);

        /// <summary>
        /// [PUT] تحديث إعداد نظام محدد
        /// </summary>
        Task<SystemSettingDto> UpdateSystemSettingAsync(UpdateSystemSettingRequest request);

        /// <summary>
        /// [PUT] تحديث تفاصيل فترة زمنية
        /// </summary>
        Task<TimePeriodDto> UpdateTimePeriodAsync(UpdateTimePeriodRequest request);
    }
}