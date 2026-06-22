using Moeen.Shared.Responses.Mobile;

namespace Moeen.App.Services.Abstractions
{
    public interface IParentLibraryService
    {
        Task<ParentLibraryResponse> GetBooksAsync(int pageNumber = 1, int pageSize = 20, string? query = null);
    }
}
