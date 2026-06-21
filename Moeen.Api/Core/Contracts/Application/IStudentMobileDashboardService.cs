using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IStudentMobileDashboardService
    {
        Task<GeneralResponse> GetDashboardAsync(Guid studentId);
    }
}
