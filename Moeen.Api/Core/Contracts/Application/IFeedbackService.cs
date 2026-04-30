using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses.Feedback;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IFeedbackService
    {
        /// <summary>
        /// استقبال شكوى جديدة
        /// </summary>
        Task<SubmitComplaintResponse> SubmitComplaintAsync(SubmitComplaintRequest request);

        /// <summary>
        /// استقبال اقتراح جديد
        /// </summary>
        Task<SubmitSuggestionResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request);

        /// <summary>
        /// إدارة الشكاوى والاقتراحات (الرد عليها)
        /// </summary>
        Task<ManageFeedbackResponse> ManageFeedbacksAsync(ManageFeedbackRequest request);

        /// <summary>
        /// تحديث حالة الشكوى
        /// </summary>
        Task<ManageFeedbackResponse> UpdateComplaintStatusAsync(UpdateComplaintStatusRequest request);
    }
}