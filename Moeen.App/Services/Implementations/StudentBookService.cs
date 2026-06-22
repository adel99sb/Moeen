using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Library;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class StudentBookService : IStudentBookService
    {
        private readonly StudentBookApiClient _apiClient;

        public StudentBookService(StudentBookApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<BookResponseDto>> GetBooksAsync()
        {
            var response = await _apiClient.GetBooksAsync();
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب كتب المكتبة.");

            var json = JsonSerializer.Serialize(response.Data);
            var books = JsonSerializer.Deserialize<List<BookResponseDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return books ?? new List<BookResponseDto>();
        }
    }
}
