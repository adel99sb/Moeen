using Moeen.Shared.Constants;
using Moeen.Shared.Responses.Mobile;

namespace Moeen.App.Services.Abstractions
{
    public interface IStudentProgressService
    {
        Task<StudentProgressResponse> GetMyProgressAsync(DateTime? from = null, DateTime? to = null, ProgressRecordType? type = null);
    }
}
