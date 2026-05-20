using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mosuq;
using System.Text.Json;

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

        public async Task<List<MosquDto>> GetAllMosqus(GetAllMosqusRequest request)
        {
            var res = await _apiClient.GetAllMosqusAsync(request);
            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Unknown error");

            var json = JsonSerializer.Serialize(res.Data);

            var data = JsonSerializer.Deserialize<List<MosquDto>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            return data;
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

        public async Task<MosquDto> GetMosqueByIdAsync(Guid mosqueId)
        {
            var res = await _apiClient.GetMosqueByIdAsync(mosqueId);
            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Unknown error");

            var json = JsonSerializer.Serialize(res.Data);

            var data = JsonSerializer.Deserialize<MosquDto>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            return data;
        }
    }
}
