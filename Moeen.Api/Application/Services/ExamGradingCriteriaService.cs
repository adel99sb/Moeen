using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ExamGrading;
using Moeen.Shared.Responses.ExamGrading;

namespace Moeen.Api.Application.Services
{
    public class ExamGradingCriteriaService : IExamGradingCriteriaService
    {
        public Task<DeleteGradingCriteriaResponse> DeleteGradingCriteriaAsync(DeleteGradingCriteriaRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GradingCriteriaDto> GetCriteriaByIdAsync(GetCriteriaByIdRequest request)
        { 
            throw new NotImplementedException();
        }

        public Task<GetGradingCriteriaResponse> GetGradingCriteriaAsync(GetGradingCriteriaRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GradingCriteriaDto> SetGradingCriteriaAsync(SetGradingCriteriaRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
