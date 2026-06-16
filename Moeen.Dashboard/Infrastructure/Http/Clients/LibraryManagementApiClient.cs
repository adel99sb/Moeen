using Microsoft.AspNetCore.WebUtilities;
using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class LibraryManagementApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<LibraryManagementApiClient> _logger;

        public LibraryManagementApiClient(HttpClient http, ILogger<LibraryManagementApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<GeneralResponse> GetAllBooksAsync(GetAllBooksRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "Category", request.Category },
                { "Author", request.Author },
                { "Language", request.Language },
                { "PageNumber", request.PageNumber.ToString() },
                { "PageSize", request.PageSize.ToString() }
            };

            var url = QueryHelpers.AddQueryString(ApiRoutes.LibraryBooksRoute, query);
            var response = await _http.GetAsync(url);
            return await ReadGeneralResponseAsync(response, "GetAllBooks");
        }

        public async Task<GeneralResponse> AddBookAsync(AddBookRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.LibraryBooksRoute, request);
            return await ReadGeneralResponseAsync(response, "AddBook");
        }

        public async Task<GeneralResponse> UpdateBookAsync(UpdateBookRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.LibraryBooksRoute, request);
            return await ReadGeneralResponseAsync(response, "UpdateBook");
        }

        public async Task<GeneralResponse> DeleteBookAsync(Guid bookId)
        {
            var response = await _http.DeleteAsync(ApiRoutes.LibraryBookByIdRoute(bookId));
            return await ReadGeneralResponseAsync(response, "DeleteBook");
        }

        private async Task<GeneralResponse> ReadGeneralResponseAsync(HttpResponseMessage response, string operation)
        {
            var body = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType;

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = TryReadApiErrorMessage(body);
                _logger.LogWarning(
                    "Library API request failed. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, BodyStart={BodyStart}",
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
                _logger.LogWarning(ex, "Library API returned non-JSON response. Operation={Operation}", operation);
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
