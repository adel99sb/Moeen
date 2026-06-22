using Moeen.Shared.Responses.Mobile;

namespace Moeen.App.Services.Abstractions
{
    public interface IParentProfileService
    {
        Task<ParentProfileResponse> GetMyProfileAsync();
        Task SubmitNoteAsync(string content);
    }
}
