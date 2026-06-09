using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mosuq;
using System.Text.Json;

namespace Moeen.Dashboard.Services.Implementations

{
    public class MosquService : IMosquService
    {
        private readonly MosquApiClient _MapiClient;
        private readonly AuthApiClient _UapiClient;


        public MosquService(MosquApiClient apiClient, AuthApiClient uapiClient)
        {
            _MapiClient = apiClient;
            _UapiClient = uapiClient;
        }

        public async Task<GeneralResponse> AddMosqu(AddMosquReq req)
        {
            return await _MapiClient.AddMosquAsync(req);
        }

        public async Task<List<MosquDto>> GetAllMosqus(GetAllMosqusRequest request)
        {
            var res = await _MapiClient.GetAllMosqusAsync(request);
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
            return await _MapiClient.GetCirclesByMosqueAsync(request);
        }

        public async Task<GeneralResponse> GetTeachersByMosqueAsync(GetTeachersByMosqueRequest request)
        {
            return await _MapiClient.GetTeachersByMosqueAsync(request);
        }

        public async Task<GeneralResponse> GetMosqueStatisticsAsync(GetMosqueStatisticsRequest request)
        {
            return await _MapiClient.GetMosqueStatisticsAsync(request);
        }

        public async Task<GeneralResponse> UpdateMosqueInfoAsync(UpdateMosqueInfoRequest request)
        {
            return await _MapiClient.UpdateMosqueInfoAsync(request);
        }



        public async Task<GeneralResponse> DeleteMosqueAsync(DeleteMosqueRequest request)
        {
            return await _MapiClient.DeleteMosqueAsync(request);
        }

        public async Task<GeneralResponse> UnassignMosqueAdminAsync(UnassignMosqueAdminRequest request)
        {
            return await _MapiClient.UnassignMosqueAdminAsync(request);
        }

        public async Task<MosquDto> GetMosqueByIdAsync(Guid mosqueId)
        {
            var res = await _MapiClient.GetMosqueByIdAsync(mosqueId);
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

        public async Task<GeneralResponse> AssignMosqueAdminAsync(AssignMosqueAdminRequest request,RegisterRequest register)
        {
            var Ures =   await _UapiClient.Register(register);
            if (Ures.Success)
            {
                var asRes = await _MapiClient.MosquAssignAdmin(request);
                    return asRes;
            }
            return Ures;
        }
    }
}