using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IParentMobileDashboardService
    {
        Task<GeneralResponse> GetDashboardAsync(Guid parentId, Guid? childId);
    }
}
