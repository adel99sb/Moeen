using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class AuthApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<AuthApiClient> _logger;

        public AuthApiClient(HttpClient http, ILogger<AuthApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        // ================= LOGIN =================

        public async Task<GeneralResponse> Login(LoginRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(ApiRoutes.LoginRoute, request);
                return await ReadGeneralResponseAsync(response, "تسجيل الدخول");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Login API connection failed. BaseAddress={BaseAddress}, Route={Route}", _http.BaseAddress, ApiRoutes.LoginRoute);
                return GeneralResponse.InternalError("تعذر الاتصال بسيرفر الـ API. تأكد أن الـ API يعمل وأن رابط الاتصال صحيح.");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Login API request timed out. BaseAddress={BaseAddress}, Route={Route}", _http.BaseAddress, ApiRoutes.LoginRoute);
                return GeneralResponse.InternalError("انتهت مهلة الاتصال أثناء تسجيل الدخول. حاول مرة ثانية أو تأكد من تشغيل الـ API.");
            }
        }

        // ================= REGISTER =================

        public async Task<GeneralResponse> Register(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.RegisterRoute, request);
            return await ReadGeneralResponseAsync(response, "إنشاء الحساب");
        }

        // ================= USERS =================

        public async Task<GeneralResponse> SearchUsers(
            PaginationRequest request,
            string? keyword = null)
        {
            string url = ApiRoutes.SearchUsersRoute;

            if (!string.IsNullOrWhiteSpace(keyword))
                url += $"?keyword={Uri.EscapeDataString(keyword)}";

            var response = await _http.PostAsJsonAsync(url, request);
            return await ReadGeneralResponseAsync(response, "البحث عن المستخدمين");
        }

        public async Task<GeneralResponse> GetUserById(Guid id)
        {
            var response = await _http.GetAsync(ApiRoutes.GetUserById(id));
            return await ReadGeneralResponseAsync(response, "جلب بيانات المستخدم");
        }

        // ================= EMAIL =================

        public async Task<GeneralResponse> ChangeEmail(string email)
        {
            var response = await _http.PutAsJsonAsync(
                ApiRoutes.ChangeEmailRoute,
                email);

            return await ReadGeneralResponseAsync(response, "تعديل البريد الإلكتروني");
        }

        // ================= PASSWORD =================

        public async Task<GeneralResponse> SendResetUrl(
            SendPasswordResetUrlRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                ApiRoutes.SendResetUrlRoute,
                request);

            return await ReadGeneralResponseAsync(response, "إرسال رابط إعادة كلمة المرور");
        }

        public async Task<GeneralResponse> ResetPassword(
            ChangePasswordRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                ApiRoutes.ResetPasswordRoute,
                request);

            return await ReadGeneralResponseAsync(response, "إعادة تعيين كلمة المرور");
        }

        // ================= VERIFICATION =================

        public async Task<GeneralResponse> SendVerifyCode(
            SendVerifyEmailCodeRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                ApiRoutes.SendVerifyCodeRoute,
                request);

            return await ReadGeneralResponseAsync(response, "إرسال كود التحقق");
        }

        public async Task<GeneralResponse> VerifyEmail(
            VerifyEmailRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                ApiRoutes.VerifyEmailRoute,
                request);

            return await ReadGeneralResponseAsync(response, "التحقق من البريد الإلكتروني");
        }

        private async Task<GeneralResponse> ReadGeneralResponseAsync(HttpResponseMessage response, string operationName)
        {
            var body = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "unknown";
            var statusCode = (int)response.StatusCode;

            if (string.IsNullOrWhiteSpace(body))
            {
                _logger.LogError(
                    "API returned an empty response. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, Url={Url}",
                    operationName,
                    statusCode,
                    contentType,
                    response.RequestMessage?.RequestUri);

                return BuildFailureResponse(
                    response.StatusCode,
                    $"لم يرجع السيرفر أي بيانات أثناء عملية {operationName}. حالة الرد: {statusCode}.");
            }

            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                if (root.ValueKind != JsonValueKind.Object || !LooksLikeGeneralResponse(root))
                {
                    _logger.LogError(
                        "API returned JSON that does not match GeneralResponse. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, Url={Url}, BodyPreview={BodyPreview}",
                        operationName,
                        statusCode,
                        contentType,
                        response.RequestMessage?.RequestUri,
                        Preview(body));

                    return BuildFailureResponse(
                        response.StatusCode,
                        $"رجع السيرفر رداً غير متوقع أثناء عملية {operationName}. حالة الرد: {statusCode}.");
                }

                var result = root.Deserialize<GeneralResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null)
                    return result;

                _logger.LogError(
                    "API response deserialized to null. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, Url={Url}, BodyPreview={BodyPreview}",
                    operationName,
                    statusCode,
                    contentType,
                    response.RequestMessage?.RequestUri,
                    Preview(body));

                return BuildFailureResponse(
                    response.StatusCode,
                    $"رجع السيرفر رداً فارغاً أثناء عملية {operationName}.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "API returned a non JSON response. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, Url={Url}, BodyPreview={BodyPreview}",
                    operationName,
                    statusCode,
                    contentType,
                    response.RequestMessage?.RequestUri,
                    Preview(body));

                return BuildFailureResponse(
                    response.StatusCode,
                    $"رجع السيرفر رداً غير مفهوم أثناء عملية {operationName}. حالة الرد: {statusCode}.");
            }
        }

        private static bool LooksLikeGeneralResponse(JsonElement root)
        {
            return HasProperty(root, "success")
                || HasProperty(root, "Success")
                || HasProperty(root, "message")
                || HasProperty(root, "Message")
                || HasProperty(root, "statusCode")
                || HasProperty(root, "StatusCode");
        }

        private static bool HasProperty(JsonElement root, string name)
        {
            return root.TryGetProperty(name, out _);
        }

        private static GeneralResponse BuildFailureResponse(HttpStatusCode statusCode, string message)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => GeneralResponse.BadRequest(message),
                HttpStatusCode.Unauthorized => GeneralResponse.Unauthorized(message),
                HttpStatusCode.NotFound => GeneralResponse.NotFound(message),
                >= HttpStatusCode.InternalServerError => GeneralResponse.InternalError(message),
                _ => new GeneralResponse(message, false, (int)statusCode)
            };
        }

        private static string Preview(string body)
        {
            const int maxLength = 500;
            var compact = body.Replace("\r", " ").Replace("\n", " ").Trim();
            return compact.Length <= maxLength ? compact : compact[..maxLength];
        }
    }
}
