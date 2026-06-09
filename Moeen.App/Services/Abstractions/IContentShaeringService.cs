using Moeen.Shared.Responses.ContentSharing;

namespace Moeen.App.Services.Abstractions
{
    public interface IContentShaeringService
    {
        Task<List<PostDto>> GetAllAsync();
    }
}
