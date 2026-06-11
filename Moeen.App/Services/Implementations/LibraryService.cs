using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Library;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class LibraryService : ILibraryService
    {
        private readonly BooksApiClient _apiClient;

        public LibraryService(BooksApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<BookResponseDto>> GetAllBooksAsync()
        {
            var res = await _apiClient.GetAllAsync();
            if (res == null || !res.Success)
                return new List<BookResponseDto>();

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<List<BookResponseDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data ?? new List<BookResponseDto>();
        }

        public async Task<BookResponseDto?> GetBookByIdAsync(Guid bookId)
        {
            var res = await _apiClient.GetByIdAsync(bookId);
            if (res == null || !res.Success)
                return null;

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<BookResponseDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data;
        }
    }
}
