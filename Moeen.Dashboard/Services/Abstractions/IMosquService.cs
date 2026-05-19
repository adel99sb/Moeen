using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;
namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IMosquService
    {
        Task<GeneralResponse> AddMosqu(AddMosquReq req);
        Task<GeneralResponse> GetAllMosqus(GetAllMosqusRequest request);
        Task<GeneralResponse> GetMosqueByIdAsync(Guid mosqueId);
        Task<GeneralResponse> GetCirclesByMosqueAsync(GetCirclesByMosqueRequest request);
        Task<GeneralResponse> GetTeachersByMosqueAsync(GetTeachersByMosqueRequest request);
        Task<GeneralResponse> GetMosqueStatisticsAsync(GetMosqueStatisticsRequest request);
        Task<GeneralResponse> UpdateMosqueInfoAsync(UpdateMosqueInfoRequest request);
        Task<GeneralResponse> AssignMosqueAdminAsync(AssignMosqueAdminRequest request);
        Task<GeneralResponse> DeleteMosqueAsync(DeleteMosqueRequest request);
        Task<GeneralResponse> UnassignMosqueAdminAsync(UnassignMosqueAdminRequest request);
    }
}
