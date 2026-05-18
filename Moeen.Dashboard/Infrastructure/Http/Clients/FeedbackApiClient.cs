using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;
namespace Moeen.Dashboard.Infrastructure.Http.Clients

{
    public class FeedbackApiClient
    {
        private readonly HttpClient _httpClient;

        public FeedbackApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.SubmitComplaintAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر.");
        }

        public async Task<GeneralResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.SubmitSuggestionAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر.");
        }

        public async Task<GeneralResponse> ManageFeedbackAsync(ManageFeedbackRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiRoutes.ManageFeedbackAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر.");
        }

        public async Task<GeneralResponse> UpdateComplaintStatusAsync(UpdateComplaintStatusRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiRoutes.UpdateComplaintStatusAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر.");
        }

        public async Task<GeneralResponse> UpdateSuggestionStatusAsync(UpdateSuggestionStatusRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiRoutes.UpdateSuggestionStatusAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر.");
        }

        public async Task<GeneralResponse> GetComplaintsAsync(PaginationRequest request)
        {
            // تحويل قيم الـ Pagination إلى Query Strings لأن الـ Controller يستقبلها عبر [FromQuery]
            var url = $"{ApiRoutes.GetComplaintsAsyncRoute}?page={request.Page}&pageSize={request.PageSize}";
            return await _httpClient.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("فشل استرجاع البيانات.");
        }

        public async Task<GeneralResponse> GetSuggestionsAsync(PaginationRequest request)
        {
            // تحويل قيم الـ Pagination إلى Query Strings لأن الـ Controller يستقبلها عبر [FromQuery]
            var url = $"{ApiRoutes.GetSuggestionsAsyncRoute}?page={request.Page}&pageSize={request.PageSize}";
            return await _httpClient.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("فشل استرجاع البيانات.");
        }
    }
}
