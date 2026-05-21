using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;
using Moeen.Shared.Requests;
using System;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IFeedbackService
    {
        /// <summary>
        /// استقبال شكوى جديدة
        /// </summary>
        Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request);

        /// <summary>
        /// استقبال اقتراح جديد
        /// </summary>
        Task<GeneralResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request);

        /// <summary>
        /// إدارة الشكاوى والاقتراحات (الرد عليها)
        /// </summary>
        Task<GeneralResponse> ManageFeedbacksAsync(ManageFeedbackRequest request);

        /// <summary>
        /// تحديث حالة الشكوى
        /// </summary>
        Task<GeneralResponse> UpdateComplaintStatusAsync(UpdateComplaintStatusRequest request);

        /// <summary>
        /// استرجاع قائمة الشكاوى مع ترقيم
        /// </summary>
        Task<GeneralResponse> GetComplaintsAsync(PaginationRequest request);

        /// <summary>
        /// استرجاع قائمة الاقتراحات مع ترقيم
        /// </summary>
        Task<GeneralResponse> GetSuggestionsAsync(PaginationRequest request);

        /// <summary>
        /// تحديث حالة الاقتراح
        /// </summary>
        Task<GeneralResponse> UpdateSuggestionStatusAsync(UpdateSuggestionStatusRequest request);

        /// <summary>
        /// حذف شكوى أو اقتراح بشكل دائم
        /// </summary>
        Task<GeneralResponse> DeleteComplaintAsync(Guid complaintId);
    }
}