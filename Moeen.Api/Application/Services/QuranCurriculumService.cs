using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.QuranCurriculum;
using Moeen.Shared.Responses.QuranCurriculum;

namespace Moeen.Api.Application.Services
{
    public class QuranCurriculumService : IQuranCurriculumService
    {
        public Task<GetJuzListResponse> GetJuzListAsync(GetJuzListRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetNewMemorizationPagesResponse> GetNewMemorizationPagesAsync(GetNewMemorizationPagesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetPagesByJuzResponse> GetPagesByJuzAsync(GetPagesByJuzRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<InitializeQuranResponse> InitializeQuranAsync(InitializeQuranRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
