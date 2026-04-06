using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Memorization;
using Moeen.Api.Shared.Responses.Memorization;

namespace Moeen.Api.Application.Services
{
    public class MemorizationService : IMemorizationService
    {
        public Task<GetLastMemorizedPageResponse> GetLastMemorizedPageAsync(GetLastMemorizedPageRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RecordPageMemorizationResponse> RecordNewPageMemorizationAsync(RecordPageMemorizationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BatchResult> RecordNewPagesBatchAsync(RecordPagesBatchRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
