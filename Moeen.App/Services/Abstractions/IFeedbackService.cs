using Moeen.Shared.Requests.Feedback;

namespace Moeen.App.Services.Abstractions
{
    public interface IFeedbackService
    {
        Task SubmitComplaintAsync(SubmitComplaintRequest request);
    }
}
