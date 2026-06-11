using Moeen.Shared.Responses.Library;

namespace Moeen.App.Services.Abstractions
{
    public interface ILibraryService
    {
        Task<List<BookResponseDto>> GetAllBooksAsync();
        Task<BookResponseDto?> GetBookByIdAsync(Guid bookId);
    }
}
