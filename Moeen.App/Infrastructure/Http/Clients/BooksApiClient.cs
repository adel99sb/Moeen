using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.App.Infrastructure.Http.Clients
{
    public class BooksApiClient
    {
        private readonly HttpClient _httpClient;
        public BooksApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(ApiRoutes.GetAllBooksRoute);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> GetByIdAsync(Guid bookId)
        {
            var url = string.Format(ApiRoutes.GetBookByIdRoute, bookId);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
