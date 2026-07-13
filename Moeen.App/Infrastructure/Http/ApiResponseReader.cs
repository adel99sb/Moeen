using Moeen.Shared.Responses;
using System.Net;
using System.Text.Json;

namespace Moeen.App.Infrastructure.Http
{
    public static class ApiResponseReader
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<GeneralResponse> ReadGeneralResponseAsync(
            HttpResponseMessage response,
            string fallbackMessage)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return response.IsSuccessStatusCode
                    ? GeneralResponse.Ok(fallbackMessage)
                    : BuildFailureResponse(
                        response.StatusCode,
                        $"{fallbackMessage} HTTP {(int)response.StatusCode}: {response.ReasonPhrase ?? "No response body"}");
            }

            try
            {
                var result = JsonSerializer.Deserialize<GeneralResponse>(body, JsonOptions);
                return result ?? BuildFailureResponse(response.StatusCode, fallbackMessage);
            }
            catch (JsonException)
            {
                return BuildFailureResponse(
                    response.StatusCode,
                    $"{fallbackMessage} رجع السيرفر رداً غير متوقع. HTTP {(int)response.StatusCode}.");
            }
        }

        public static GeneralResponse BuildFailureResponse(HttpStatusCode statusCode, string message)
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
    }
}
