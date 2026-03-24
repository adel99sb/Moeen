using Moeen.Api.Shared.Requests.SystemConfiguration;
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
    }
}