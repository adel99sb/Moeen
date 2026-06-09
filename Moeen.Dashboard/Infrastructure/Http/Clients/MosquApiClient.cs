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
            string quryString = $"?Page={request.Page}&PageSize={request.PageSize}";
            var response = await _httpClient.GetAsync(ApiRoutes.GetAllMosqusAsyncRoute + quryString);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetMosqueByIdAsync(Guid mosqueId)
        {
            string quryString = $"?mosqueId={mosqueId}";
            var response = await _httpClient.GetAsync(ApiRoutes.GetMosqueByIdAsyncRoute + quryString);
            var res = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return res;
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

        public async Task<GeneralResponse> MosquAssignAdmin(AssignMosqueAdminRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiRoutes.MosquAssignAdmin, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }


        public async Task<GeneralResponse> DeleteMosqueAsync(DeleteMosqueRequest request)
        {
            var quryString = $"?MosqueId={request.MosqueId}";

            var response = await _httpClient.DeleteAsync(ApiRoutes.DeleteMosqueAsyncRoute + quryString);
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