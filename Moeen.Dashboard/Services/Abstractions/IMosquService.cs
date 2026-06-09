using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mosuq;
namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IMosquService
    {
        Task<GeneralResponse> AddMosqu(AddMosquReq req);
        Task<List<MosquDto>> GetAllMosqus(GetAllMosqusRequest request);
        Task<MosquDto> GetMosqueByIdAsync(Guid mosqueId);
        Task<GeneralResponse> GetCirclesByMosqueAsync(GetCirclesByMosqueRequest request);
        Task<GeneralResponse> GetTeachersByMosqueAsync(GetTeachersByMosqueRequest request);
        Task<GeneralResponse> GetMosqueStatisticsAsync(GetMosqueStatisticsRequest request);
        Task<GeneralResponse> UpdateMosqueInfoAsync(UpdateMosqueInfoRequest request);
        Task<GeneralResponse> AssignMosqueAdminAsync(AssignMosqueAdminRequest request, RegisterRequest register);
        Task<GeneralResponse> DeleteMosqueAsync(DeleteMosqueRequest request);
        Task<GeneralResponse> UnassignMosqueAdminAsync(UnassignMosqueAdminRequest request);
    }
}