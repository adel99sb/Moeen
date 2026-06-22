using Moeen.Shared.Constants;
using Moeen.Shared.Responses.Mobile;

namespace Moeen.App.Services.Abstractions
{
    public interface IParentProgressService
    {
        Task<ParentProgressResponse> GetMyChildProgressAsync(Guid? childId = null, DateTime? from = null, DateTime? to = null, ProgressRecordType? type = null);
    }
}
