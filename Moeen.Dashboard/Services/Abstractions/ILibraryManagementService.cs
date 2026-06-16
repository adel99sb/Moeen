using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface ILibraryManagementService
    {
        Task<GeneralResponse> GetAllBooksAsync(GetAllBooksRequest request);
        Task<GeneralResponse> AddBookAsync(AddBookRequest request);
        Task<GeneralResponse> UpdateBookAsync(UpdateBookRequest request);
        Task<GeneralResponse> DeleteBookAsync(Guid bookId);
    }
}
