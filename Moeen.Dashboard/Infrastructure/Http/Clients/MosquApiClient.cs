using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients

{
    public class MosquApiClient
    {
        private readonly HttpClient _httpClient;

        public MosquApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> AddMosquAsync(AddMosquReq req)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.AddMosquAsyncRoute, req);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetAllMosqusAsync(GetAllMosqusRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.GetAllMosqusAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetMosqueByIdAsync(Guid mosqueId)
        {
            var response = await _httpClient.GetAsync(ApiRoutes.GetMosqueByIdAsyncRoute);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetCirclesByMosqueAsync(GetCirclesByMosqueRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.GetCirclesByMosqueAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetTeachersByMosqueAsync(GetTeachersByMosqueRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.GetTeachersByMosqueAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetMosqueStatisticsAsync(GetMosqueStatisticsRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.GetMosqueStatisticsAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> UpdateMosqueInfoAsync(UpdateMosqueInfoRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiRoutes.UpdateMosqueInfoAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> AssignMosqueAdminAsync(AssignMosqueAdminRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiRoutes.AssignMosqueAdminAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> DeleteMosqueAsync(DeleteMosqueRequest request)
        {
            // بما أن الـ Controller يتوقع طلب بـ Body للحذف، نستخدم SendAsync لتمرير الـ Json مع الـ Delete
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_httpClient.BaseAddress + ApiRoutes.DeleteMosqueAsyncRoute),
                Content = JsonContent.Create(request)
            };

            var response = await _httpClient.SendAsync(requestMessage);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> UnassignMosqueAdminAsync(UnassignMosqueAdminRequest request)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_httpClient.BaseAddress + ApiRoutes.UnassignMosqueAdminAsyncRoute),
                Content = JsonContent.Create(request)
            };

            var response = await _httpClient.SendAsync(requestMessage);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
