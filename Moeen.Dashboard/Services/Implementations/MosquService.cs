using System;
using System.Threading.Tasks;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations

{
    public class MosquService : IMosquService
    {
        private readonly MosquApiClient _apiClient;

        public MosquService(MosquApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<GeneralResponse> AddMosqu(AddMosquReq req)
        {
            return await _apiClient.AddMosquAsync(req);
        }

        public async Task<GeneralResponse> GetAllMosqus(GetAllMosqusRequest request)
        {
            return await _apiClient.GetAllMosqusAsync(request);
        }

        public async Task<GeneralResponse> GetMosqueByIdAsync(Guid mosqueId)
        {
            return await _apiClient.GetMosqueByIdAsync(mosqueId);
        }

        public async Task<GeneralResponse> GetCirclesByMosqueAsync(GetCirclesByMosqueRequest request)
        {
            return await _apiClient.GetCirclesByMosqueAsync(request);
        }

        public async Task<GeneralResponse> GetTeachersByMosqueAsync(GetTeachersByMosqueRequest request)
        {
            return await _apiClient.GetTeachersByMosqueAsync(request);
        }

        public async Task<GeneralResponse> GetMosqueStatisticsAsync(GetMosqueStatisticsRequest request)
        {
            return await _apiClient.GetMosqueStatisticsAsync(request);
        }

        public async Task<GeneralResponse> UpdateMosqueInfoAsync(UpdateMosqueInfoRequest request)
        {
            return await _apiClient.UpdateMosqueInfoAsync(request);
        }

        public async Task<GeneralResponse> AssignMosqueAdminAsync(AssignMosqueAdminRequest request)
        {
            return await _apiClient.AssignMosqueAdminAsync(request);
        }

        public async Task<GeneralResponse> DeleteMosqueAsync(DeleteMosqueRequest request)
        {
            return await _apiClient.DeleteMosqueAsync(request);
        }

        public async Task<GeneralResponse> UnassignMosqueAdminAsync(UnassignMosqueAdminRequest request)
        {
            return await _apiClient.UnassignMosqueAdminAsync(request);
        }
    }
}
