using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Library;
using Moeen.Api.Shared.Responses.Library;

namespace Moeen.Api.Application.Services
{
    public class LibraryService : ILibraryService
    {
        public Task<AddBookResponse> AddBookAsync(AddBookRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SearchLibraryResponse> SearchLibraryAsync(SearchLibraryRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateBookResponse> UpdateBookInfoAsync(UpdateBookRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
