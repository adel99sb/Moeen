using Moeen.Shared.Responses.Library;

namespace Moeen.App.Services.Abstractions
{
    public interface IStudentBookService
    {
        Task<List<BookResponseDto>> GetBooksAsync();
    }
}
