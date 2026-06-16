using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations
{
    public class LibraryManagementService : ILibraryManagementService
    {
        private readonly LibraryManagementApiClient _client;

        public LibraryManagementService(LibraryManagementApiClient client)
        {
            _client = client;
        }

        public async Task<GeneralResponse> GetAllBooksAsync(GetAllBooksRequest request)
            => await _client.GetAllBooksAsync(request);

        public async Task<GeneralResponse> AddBookAsync(AddBookRequest request)
            => await _client.AddBookAsync(request);

        public async Task<GeneralResponse> UpdateBookAsync(UpdateBookRequest request)
            => await _client.UpdateBookAsync(request);

        public async Task<GeneralResponse> DeleteBookAsync(Guid bookId)
            => await _client.DeleteBookAsync(bookId);
    }
}
