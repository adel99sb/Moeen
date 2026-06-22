using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Library;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class ParentLibraryService : IParentLibraryService
    {
        private readonly ParentLibraryApiClient _apiClient;
        private readonly AppSessionService _session;

        public ParentLibraryService(ParentLibraryApiClient apiClient, AppSessionService session)
        {
            _apiClient = apiClient;
            _session = session;
        }

        public async Task<ParentLibraryResponse> GetBooksAsync(int pageNumber = 1, int pageSize = 20, string? query = null)
        {
            var safePage = Math.Max(pageNumber, 1);
            var safePageSize = Math.Max(pageSize, 1);
            var response = await _apiClient.GetBooksAsync(_session.AuthToken ?? string.Empty, safePage, safePageSize, query);

            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "فشل جلب كتب المكتبة.");

            var json = JsonSerializer.Serialize(response.Data);
            var books = JsonSerializer.Deserialize<List<BookResponseDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<BookResponseDto>();

            var totalCount = response.TotalCount ?? books.Count;
            return new ParentLibraryResponse
            {
                Query = query?.Trim() ?? string.Empty,
                Page = response.Page ?? safePage,
                PageSize = response.PageSize ?? safePageSize,
                TotalCount = totalCount,
                TotalPages = safePageSize > 0 ? (int)Math.Ceiling(totalCount / (double)safePageSize) : 1,
                Books = books
            };
        }
    }
}
