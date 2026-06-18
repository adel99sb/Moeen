using Moeen.Shared.Requests.HalqaQuery;
using Moeen.Shared.Responses.Halqa;
using Moeen.Shared.Responses.HalqaQuery;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IHalqaQueryService
    {
        Task<HalqaDto> GetHalqaByIdAsync(GetHalqaByIdRequest request);
        Task<List<HalqaDto>> GetAllHalqasAsync(Guid? mosqueId = null);
        Task<List<HalqaAssignmentStudentOptionDto>> GetAssignmentStudentsAsync(Guid? halqaId = null);
        Task<HalqaStudentsResponse> GetHalqaStudentsAsync(GetHalqaStudentsRequest request);
        Task<HalqaStudentsCountResponse> GetHalqaStudentsCountAsync(GetHalqaStudentsCountRequest request);
        Task<HalqaStatisticsDto> GetHalqaStatisticsAsync(GetHalqaStatisticsRequest request);
        Task<HalqaAttendanceReportResponse> GetHalqaAttendanceReportAsync(GetHalqaAttendanceReportRequest request);
    }
}
