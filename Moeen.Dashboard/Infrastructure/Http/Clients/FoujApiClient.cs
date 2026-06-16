using Microsoft.AspNetCore.WebUtilities;
using Moeen.Shared.Requests.Fouj;
using Moeen.Shared.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class FoujApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<FoujApiClient> _logger;

        public FoujApiClient(HttpClient http, ILogger<FoujApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<GeneralResponse> GetAllAsync(GetAllFoujsRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "MosqueId", request.MosqueId?.ToString() }
            };

            var url = QueryHelpers.AddQueryString(ApiRoutes.FoujRoute, query);
            var response = await _http.GetAsync(url);
            return await ReadGeneralResponseAsync(response, "GetAllFoujs");
        }

        public async Task<GeneralResponse> CreateAsync(CreateFoujRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.FoujRoute, request);
            return await ReadGeneralResponseAsync(response, "CreateFouj");
        }

        public async Task<GeneralResponse> UpdateAsync(UpdateFoujRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.FoujByIdRoute(request.FoujId), request);
            return await ReadGeneralResponseAsync(response, "UpdateFouj");
        }

        public async Task<GeneralResponse> DeleteAsync(Guid foujId)
        {
            var response = await _http.DeleteAsync(ApiRoutes.FoujByIdRoute(foujId));
            return await ReadGeneralResponseAsync(response, "DeleteFouj");
        }

        private async Task<GeneralResponse> ReadGeneralResponseAsync(HttpResponseMessage response, string operation)
        {
            var body = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType;

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = TryReadApiErrorMessage(body);
                _logger.LogWarning(
                    "Fouj API request failed. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, BodyStart={BodyStart}",
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
                _logger.LogWarning(ex, "Fouj API returned non-JSON response. Operation={Operation}", operation);
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
