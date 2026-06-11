using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Requests.Feedback;

namespace Moeen.App.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly FeedbackApiClient _apiClient;

        public FeedbackService(FeedbackApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task SubmitComplaintAsync(SubmitComplaintRequest request)
        {
            try
            {
                var res = await _apiClient.SubmitComplaintAsync(request);
                if (res == null || !res.Success)
                    throw new Exception(res?.Message ?? "Unknown error");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
