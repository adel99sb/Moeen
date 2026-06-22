using Moeen.Shared.Responses.Mobile;

namespace Moeen.App.Services.Abstractions
{
    public interface IParentDashboardService
    {
        Task<ParentDashboardResponse> GetMyDashboardAsync(Guid? childId = null);
    }
}
