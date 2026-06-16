using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Frontend.Services.Abstractions;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly FeedbackApiClient _apiClient;

        public FeedbackService(FeedbackApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request)
        {
            return await _apiClient.SubmitComplaintAsync(request);
        }

        public async Task<GeneralResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request)
        {
            return await _apiClient.SubmitSuggestionAsync(request);
        }

        public async Task<GeneralResponse> ManageFeedbackAsync(ManageFeedbackRequest request)
        {
            return await _apiClient.ManageFeedbackAsync(request);
        }

        public async Task<GeneralResponse> UpdateComplaintStatusAsync(UpdateComplaintStatusRequest request)
        {
            return await _apiClient.UpdateComplaintStatusAsync(request);
        }

        public async Task<GeneralResponse> UpdateSuggestionStatusAsync(UpdateSuggestionStatusRequest request)
        {
            return await _apiClient.UpdateSuggestionStatusAsync(request);
        }

        public async Task<List<ComplaintDto>> GetComplaintsAsync(PaginationRequest request)
        {
            var res = await _apiClient.GetComplaintsAsync(request);
            if (res == null || !res.Success)
                throw new Exception(res?.Message ?? "Unknown error");

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<List<ComplaintDto>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return data ?? new List<ComplaintDto>();
        }

        public async Task<GeneralResponse> GetSuggestionsAsync(PaginationRequest request)
        {
            return await _apiClient.GetSuggestionsAsync(request);
        }

        public async Task<GeneralResponse> GetComplaintsResponseAsync(PaginationRequest request)
        {
            return await _apiClient.GetComplaintsAsync(request);
        }

        public async Task<GeneralResponse> DeleteFeedbackAsync(Guid complaintId)
        {
            return await _apiClient.DeleteFeedbackAsync(complaintId);
        }
    }
}
