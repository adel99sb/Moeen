using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class PostApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;
        private readonly ITokenService _tokenService;

        public PostApiClient(HttpClient http, ITokenService tokenService)
        {
            _http = http;
            _tokenService = tokenService;
        }

        public Task<GeneralResponse?> GetAllPosts()
            => SendAuthorizedAndReadAsync(
                HttpMethod.Get,
                ApiRoutes.GetAllPostsRoute,
                operation: "جلب المنشورات");

        public Task<GeneralResponse?> PublishPost(PublishPostRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Post,
                ApiRoutes.PublishPostRoute,
                JsonContent.Create(request),
                "نشر المنشور");

        public Task<GeneralResponse?> GetHalqasBrief(string? query)
        {
            var url = ApiRoutes.GetPostHalqasBriefRoute;
            if (!string.IsNullOrWhiteSpace(query))
                url += $"?query={Uri.EscapeDataString(query)}";

            return SendAuthorizedAndReadAsync(
                HttpMethod.Get,
                url,
                operation: "جلب الحلقات");
        }

        public Task<GeneralResponse?> UpdatePost(UpdatePostRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Put,
                ApiRoutes.UpdatePostRoute(request.PostId),
                JsonContent.Create(request),
                "تعديل المنشور");

        public Task<GeneralResponse?> DeletePost(DeletePostRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Delete,
                ApiRoutes.DeletePostRoute(request.PostId),
                operation: "حذف المنشور");

        public Task<GeneralResponse?> GetPostById(GetPostByIdRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Get,
                ApiRoutes.GetPostByIdRoute(request.PostId, request.IncludeInteractions),
                operation: "جلب المنشور");

        public Task<GeneralResponse?> GetPostInteractions(GetPostInteractionsRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Get,
                ApiRoutes.GetPostInteractionsRoute(request.PostId, request.PageNumber, request.PageSize),
                operation: "جلب تفاعلات المنشور");

        public Task<GeneralResponse?> InteractWithPost(InteractWithPostRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Post,
                ApiRoutes.IntractPostRoute,
                JsonContent.Create(request),
                "التفاعل مع المنشور");

        public Task<GeneralResponse?> ManageAnnouncements(ManageAnnouncementRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Post,
                ApiRoutes.ManagAnnoucmentRoute,
                JsonContent.Create(request),
                "إدارة الإعلانات");

        public Task<GeneralResponse?> SearchContentPost(SearchContentRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Post,
                ApiRoutes.SearchPostRoute,
                JsonContent.Create(request),
                "البحث في المنشورات");

        public Task<GeneralResponse?> SearchContentGet(string query)
        {
            var url = $"{ApiRoutes.GetSearchPostRoute}?query={Uri.EscapeDataString(query ?? string.Empty)}";
            return SendAuthorizedAndReadAsync(
                HttpMethod.Get,
                url,
                operation: "البحث في المنشورات");
        }

        public Task<GeneralResponse?> DeleteOldContent(DeleteOldContentRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Post,
                ApiRoutes.DeleteOldRoute,
                JsonContent.Create(request),
                "حذف المحتوى القديم");

        public Task<GeneralResponse?> AddMultimedia(AddMultimediaRequest request)
            => SendAuthorizedAndReadAsync(
                HttpMethod.Post,
                ApiRoutes.AddMultiMediaRoute,
                JsonContent.Create(request),
                "إضافة الوسائط");

        private async Task<GeneralResponse?> SendAuthorizedAndReadAsync(
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
            return await _http.SendAsync(request);
        }

        private static async Task<GeneralResponse> ReadGeneralResponseAsync(
            HttpResponseMessage response,
            string operation)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return new GeneralResponse(
                    GetResponseErrorMessage(response.StatusCode, operation),
                    success: false,
                    statusCode: (int)response.StatusCode);
            }

            try
            {
                var parsed = JsonSerializer.Deserialize<GeneralResponse>(body, JsonOptions);
                if (parsed != null)
                    return parsed;
            }
            catch (JsonException)
            {
                // Convert invalid API payloads into a friendly failure instead of leaking a JSON parser exception to the UI.
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
