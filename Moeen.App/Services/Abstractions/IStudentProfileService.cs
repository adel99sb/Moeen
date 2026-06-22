using Moeen.Shared.Responses.Mobile;

namespace Moeen.App.Services.Abstractions
{
    public interface IStudentProfileService
    {
        Task<StudentProfileResponse> GetMyProfileAsync();
        Task SubmitNoteAsync(string content);
    }
}
