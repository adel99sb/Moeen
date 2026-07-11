using Microsoft.AspNetCore.WebUtilities;
using Moeen.Dashboard.Services.Abstractions;
using System.Net.Http.Headers;
using Moeen.Shared.Requests.Halqa;
using Moeen.Shared.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class HalqaApiClient
    {
        private readonly HttpClient _http;
        private readonly ITokenService _tokenService;
        private readonly ILogger<HalqaApiClient> _logger;

        public HalqaApiClient(HttpClient http, ITokenService tokenService, ILogger<HalqaApiClient> logger)
        {
            _http = http;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<GeneralResponse> GetAllAsync(Guid? mosqueId = null)
        {
            var query = new Dictionary<string, string?>
            {
                { "mosqueId", mosqueId?.ToString() }
            };

            var url = QueryHelpers.AddQueryString(ApiRoutes.GetAllHalqasAsyncRoute, query);
            using var response = await SendAuthorizedAsync(HttpMethod.Get, url);
            return await ReadGeneralResponseAsync(response, "GetAllHalqas");
        }

        public async Task<GeneralResponse> GetAssignmentStudentsAsync(Guid? halqaId = null)
        {
            var query = new Dictionary<string, string?>
            {
                { "halqaId", halqaId?.ToString() }
            };

            var url = QueryHelpers.AddQueryString(ApiRoutes.GetHalqaAssignmentStudentsAsyncRoute, query);
            using var response = await SendAuthorizedAsync(HttpMethod.Get, url);
            return await ReadGeneralResponseAsync(response, "GetHalqaAssignmentStudents");
        }

        public async Task<GeneralResponse> CreateAsync(CreateHalqaRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Post, ApiRoutes.CreateHalqaAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "CreateHalqa");
        }

        public async Task<GeneralResponse> UpdateAsync(UpdateHalqaRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Put, ApiRoutes.UpdateHalqaAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "UpdateHalqa");
        }

        public async Task<GeneralResponse> DeleteAsync(DeleteHalqaRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Delete, ApiRoutes.DeleteHalqaAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "DeleteHalqa");
        }

        private async Task<HttpResponseMessage> SendAuthorizedAsync(HttpMethod method, string route, HttpContent? content = null)
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

        private async Task<GeneralResponse> ReadGeneralResponseAsync(HttpResponseMessage response, string operation)
        {
            var body = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType;

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = TryReadApiErrorMessage(body);
                _logger.LogWarning(
                    "Halqa API request failed. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, BodyStart={BodyStart}",
                    operation,
                    (int)response.StatusCode,
                    contentType,
                    string.IsNullOrWhiteSpace(body) ? string.Empty : body.Length > 300 ? body[..300] : body);

                return new GeneralResponse(
                    string.IsNullOrWhiteSpace(errorMessage) ? $"فشل تنفيذ العملية. StatusCode={(int)response.StatusCode}" : errorMessage,
                    success: false,
                    statusCode: (int)response.StatusCode);
            }

            if (string.IsNullOrWhiteSpace(body))
                return GeneralResponse.Ok("تمت العملية بنجاح.");

            try
            {
                var parsed = JsonSerializer.Deserialize<GeneralResponse>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed != null)
                    return parsed;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Halqa API returned non-JSON response. Operation={Operation}", operation);
            }

            return new GeneralResponse(body.Length > 700 ? body[..700] : body, success: false, statusCode: (int)response.StatusCode);
        }

        private static string? TryReadApiErrorMessage(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return null;

            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                if (root.TryGetProperty("message", out var messageElement) && messageElement.ValueKind == JsonValueKind.String)
                    return messageElement.GetString();

                if (root.TryGetProperty("title", out var titleElement) && titleElement.ValueKind == JsonValueKind.String)
                    return titleElement.GetString();
            }
            catch (JsonException)
            {
                return body.Length > 700 ? body[..700] : body;
            }

            return body.Length > 700 ? body[..700] : body;
        }
    }
}
