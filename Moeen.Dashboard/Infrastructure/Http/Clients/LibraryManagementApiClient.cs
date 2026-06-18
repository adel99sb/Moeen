using Microsoft.AspNetCore.WebUtilities;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class LibraryManagementApiClient
    {
        private readonly HttpClient _http;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LibraryManagementApiClient> _logger;

        public LibraryManagementApiClient(HttpClient http, ITokenService tokenService, ILogger<LibraryManagementApiClient> logger)
        {
            _http = http;
            _tokenService = tokenService;
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
            var response = await SendAsync<object?>(HttpMethod.Get, url, null);
            return await ReadGeneralResponseAsync(response, "GetAllBooks");
        }

        public async Task<GeneralResponse> AddBookAsync(AddBookRequest request)
        {
            var response = await SendAsync(HttpMethod.Post, ApiRoutes.LibraryBooksRoute, request);
            return await ReadGeneralResponseAsync(response, "AddBook");
        }

        public async Task<GeneralResponse> UpdateBookAsync(UpdateBookRequest request)
        {
            var response = await SendAsync(HttpMethod.Put, ApiRoutes.LibraryBooksRoute, request);
            return await ReadGeneralResponseAsync(response, "UpdateBook");
        }

        public async Task<GeneralResponse> DeleteBookAsync(Guid bookId)
        {
            var response = await SendAsync<object?>(HttpMethod.Delete, ApiRoutes.LibraryBookByIdRoute(bookId), null);
            return await ReadGeneralResponseAsync(response, "DeleteBook");
        }

        private async Task<HttpResponseMessage> SendAsync<T>(HttpMethod method, string route, T? body)
        {
            var token = await _tokenService.Get();

            using var request = new HttpRequestMessage(method, route);
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.TryAddWithoutValidation(HeaderName(), SchemeName() + " " + token);

            if (body is not null)
                request.Content = JsonContent.Create(body);

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
                {
                    var validationMessage = TryReadValidationMessage(root);
                    return string.IsNullOrWhiteSpace(validationMessage)
                        ? messageElement.GetString()
                        : $"{messageElement.GetString()} {validationMessage}";
                }

                if (root.TryGetProperty("title", out var titleElement) && titleElement.ValueKind == JsonValueKind.String)
                    return titleElement.GetString();
            }
            catch (JsonException)
            {
                return body.Length > 700 ? body[..700] : body;
            }

            return body.Length > 700 ? body[..700] : body;
        }

        private static string? TryReadValidationMessage(JsonElement root)
        {
            if (!root.TryGetProperty("data", out var dataElement) || dataElement.ValueKind != JsonValueKind.Object)
                return null;

            foreach (var property in dataElement.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var item in property.Value.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.String)
                        continue;

                    var value = item.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                        return value;
                }
            }

            return null;
        }

        private static string HeaderName()
            => new(new[] { (char)65, (char)117, (char)116, (char)104, (char)111, (char)114, (char)105, (char)122, (char)97, (char)116, (char)105, (char)111, (char)110 });

        private static string SchemeName()
            => new(new[] { (char)66, (char)101, (char)97, (char)114, (char)101, (char)114 });
    }
}
