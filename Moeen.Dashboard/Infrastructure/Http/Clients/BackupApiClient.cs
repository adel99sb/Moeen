using System.Net.Http.Json;
using System.Text.Json;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class BackupApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<BackupApiClient> _logger;

        public BackupApiClient(HttpClient http, ILogger<BackupApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<GeneralResponse> CreateBackupAsync()
        {
            var response = await _http.PostAsync(ApiRoutes.CreateBackupRoute, content: null);
            return await ReadGeneralResponseAsync(response, "CreateBackup");
        }

        public async Task<GeneralResponse> GetLastBackupInfoAsync()
        {
            var response = await _http.GetAsync(ApiRoutes.LastBackupInfoRoute);
            return await ReadGeneralResponseAsync(response, "GetLastBackupInfo");
        }

        private async Task<GeneralResponse> ReadGeneralResponseAsync(HttpResponseMessage response, string operation)
        {
            var body = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType;

            if (string.IsNullOrWhiteSpace(body))
            {
                return response.IsSuccessStatusCode
                    ? GeneralResponse.Ok("تمت العملية بنجاح.")
                    : new GeneralResponse($"فشل تنفيذ العملية. StatusCode={(int)response.StatusCode}", false, (int)response.StatusCode);
            }

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
                _logger.LogWarning(
                    ex,
                    "Backup API returned non-JSON response. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, BodyStart={BodyStart}",
                    operation,
                    (int)response.StatusCode,
                    contentType,
                    body.Length > 300 ? body[..300] : body);
            }

            if (!response.IsSuccessStatusCode)
            {
                return new GeneralResponse(
                    string.IsNullOrWhiteSpace(body) ? $"فشل تنفيذ العملية. StatusCode={(int)response.StatusCode}" : body,
                    false,
                    (int)response.StatusCode);
            }

            return GeneralResponse.Ok("تمت العملية بنجاح.");
        }
    }
}
