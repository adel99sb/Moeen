using System.Collections.Generic;

namespace Moeen.Shared.Responses.ExamGrading
{
    public class GetGradingCriteriaResponse
    {
        public List<GradingCriteriaDto> Criteria { get; set; }
        public int TotalCount { get; set; }
    }
}