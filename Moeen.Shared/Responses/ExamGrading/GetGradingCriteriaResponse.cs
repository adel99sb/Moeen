using System.Collections.Generic;

namespace Moeen.Shared.Responses.ExamGrading
{
    public class GetGradingCriteriaResponse
    {
        public List<GradingCriteriaDto> Criteria { get; set; } = new List<GradingCriteriaDto>();
        public int TotalCount { get; set; }
    }
}