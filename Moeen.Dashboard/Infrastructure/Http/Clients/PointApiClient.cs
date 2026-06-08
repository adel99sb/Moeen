using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using Moeen.Shared.Requests.Points;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients

{
    public class PointsApiClient
    {
        private readonly HttpClient _httpClient;

        public PointsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> SetupPointsSystemAsync(SetupPointsSystemRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.SetupPointsSystemAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetStudentPointsAsync(Guid studentId)
        {
            var response = await _httpClient.GetAsync(ApiRoutes.GetStudentPointsAsyncRoute);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetStudentPointsBreakdownAsync(GetStudentPointsBreakdownRequest request)
        {
            var queryParts = new List<string>();

            if (request.FromDate.HasValue)
                queryParts.Add($"FromDate={HttpUtility.UrlEncode(request.FromDate.Value.ToString("O"))}");

            if (request.ToDate.HasValue)
                queryParts.Add($"ToDate={HttpUtility.UrlEncode(request.ToDate.Value.ToString("O"))}");

            var queryString = queryParts.Count > 0 ? "?" + string.Join("&", queryParts) : string.Empty;
            var url = ApiRoutes.GetStudentPointsBreakdownAsyncRoute(request.StudentId) + queryString;

            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetPointsLeaderboardAsync(GetLeaderboardRequest request)
        {
            // بما أن الـ Controller بيستقبل البيانات من الـ Query (FromQuery)، بنجهز الرابط مع البارامترات
            var queryString = $"?PageNumber={request.PageNumber}&PageSize={request.PageSize}";

            if (request.CircleId.HasValue && request.CircleId.Value != Guid.Empty)
            {
                queryString += $"&CircleId={request.CircleId.Value}";
            }

            var response = await _httpClient.GetAsync(ApiRoutes.GetPointsLeaderboardAsyncRoute + queryString);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> AwardPointsManuallyAsync(AwardPointsManualRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.AwardPointsManuallyAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> RemovePointsManuallyAsync(RemovePointsManualRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.RemovePointsManuallyAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> EvaluateAutomaticPointsAsync(EvaluateAutomaticPointsRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.EvaluateAutomaticPointsAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
