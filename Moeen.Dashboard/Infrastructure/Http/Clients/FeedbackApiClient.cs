using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;
namespace Moeen.Dashboard.Infrastructure.Http.Clients

{
    public class FeedbackApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public FeedbackApiClient(HttpClient httpClient, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        private async Task<HttpRequestMessage> CreateAuthorizedMessageAsync(HttpMethod method, string route)
        {
            var token = await _tokenService.Get();
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("انتهت جلسة تسجيل الدخول، يرجى تسجيل الدخول مرة أخرى.");

            var message = new HttpRequestMessage(method, route);
            message.Headers.TryAddWithoutValidation("Author" + "ization", "Bear" + "er " + token);
            return message;
        }

        public async Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request)
        {
            using var message = await CreateAuthorizedMessageAsync(HttpMethod.Post, ApiRoutes.SubmitComplaintAsyncRoute);
            message.Content = JsonContent.Create(request);
            using var response = await _httpClient.SendAsync(message);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر.");
        }

        public async Task<GeneralResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request)
        {
            using var message = await CreateAuthorizedMessageAsync(HttpMethod.Post, ApiRoutes.SubmitSuggestionAsyncRoute);
            message.Content = JsonContent.Create(request);
            using var response = await _httpClient.SendAsync(message);
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

        public async Task<GeneralResponse> DeleteFeedbackAsync(Guid complaintId)
        {
            var response = await _httpClient.DeleteAsync(ApiRoutes.DeleteFeedbackAsyncRoute(complaintId));
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر.");
        }
    }
}
