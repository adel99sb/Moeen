using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class FeedbackApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public FeedbackApiClient(HttpClient httpClient, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        public Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Post,
                ApiRoutes.SubmitComplaintAsyncRoute,
                JsonContent.Create(request),
                "إرسال الشكوى");

        public Task<GeneralResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Post,
                ApiRoutes.SubmitSuggestionAsyncRoute,
                JsonContent.Create(request),
                "إرسال الاقتراح");

        public Task<GeneralResponse> ManageFeedbackAsync(ManageFeedbackRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Put,
                ApiRoutes.ManageFeedbackAsyncRoute,
                JsonContent.Create(request),
                "إدارة الشكوى أو الاقتراح");

        public Task<GeneralResponse> UpdateComplaintStatusAsync(UpdateComplaintStatusRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Put,
                ApiRoutes.UpdateComplaintStatusAsyncRoute,
                JsonContent.Create(request),
                "تحديث حالة الشكوى");

        public Task<GeneralResponse> UpdateSuggestionStatusAsync(UpdateSuggestionStatusRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Put,
                ApiRoutes.UpdateSuggestionStatusAsyncRoute,
                JsonContent.Create(request),
                "تحديث حالة الاقتراح");

        public Task<GeneralResponse> GetComplaintsAsync(PaginationRequest request)
        {
            var url = $"{ApiRoutes.GetComplaintsAsyncRoute}?page={request.Page}&pageSize={request.PageSize}";
            return SendAuthorizedAndReadAsync(HttpMethod.Get, url, operation: "تحميل الشكاوى");
        }

        public Task<GeneralResponse> GetSuggestionsAsync(PaginationRequest request)
        {
            var url = $"{ApiRoutes.GetSuggestionsAsyncRoute}?page={request.Page}&pageSize={request.PageSize}";
            return SendAuthorizedAndReadAsync(HttpMethod.Get, url, operation: "تحميل الاقتراحات");
        }

        public Task<GeneralResponse> TransferToOwnerAsync(Guid feedbackId)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Put,
                ApiRoutes.TransferFeedbackToOwnerAsyncRoute(feedbackId),
                operation: "تحويل السجل إلى المالك");

        public Task<GeneralResponse> DeleteFeedbackAsync(Guid complaintId)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Delete,
                ApiRoutes.DeleteFeedbackAsyncRoute(complaintId),
                operation: "حذف السجل");

        private async Task<GeneralResponse> SendAuthorizedAndReadAsync(
            HttpMethod method,
            string route,
            HttpContent? content = null,
            string operation = "تنفيذ العملية")
        {
            using var response = await SendAuthorizedAsync(method, route, content);
            return await ReadGeneralResponseAsync(response, operation);
        }

        private async Task<HttpResponseMessage> SendAuthorizedAsync(
            HttpMethod method,
            string route,
            HttpContent? content = null)
        {
            var token = await _tokenService.Get();
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("انتهت جلسة تسجيل الدخول، يرجى تسجيل الدخول مرة أخرى.");

            using var request = new HttpRequestMessage(method, route)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.SendAsync(request);
        }

        private static async Task<GeneralResponse> ReadGeneralResponseAsync(
            HttpResponseMessage response,
            string operation)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<GeneralResponse>(body, JsonOptions);
                    if (parsed != null)
                        return parsed;
                }
                catch (JsonException)
                {
                    // Return a friendly API error instead of exposing JSON parsing details to the dashboard.
                }
            }

            return new GeneralResponse(
                GetResponseErrorMessage(response.StatusCode, operation),
                success: false,
                statusCode: (int)response.StatusCode);
        }

        private static string GetResponseErrorMessage(HttpStatusCode statusCode, string operation)
            => statusCode switch
            {
                HttpStatusCode.Unauthorized => "انتهت جلسة تسجيل الدخول، يرجى تسجيل الدخول مرة أخرى.",
                HttpStatusCode.Forbidden => "لا تملك صلاحية تنفيذ هذه العملية.",
                HttpStatusCode.NotFound => $"تعذر {operation}: المسار أو البيانات المطلوبة غير موجودة.",
                _ when (int)statusCode >= 500 => $"تعذر {operation}: حدث خطأ في الخادم.",
                _ => $"تعذر {operation}: أعاد الخادم استجابة غير صالحة."
            };
    }
}
