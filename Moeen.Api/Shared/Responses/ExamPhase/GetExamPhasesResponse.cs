using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.ExamPhase
{
    public class GetExamPhasesResponse
    {
        public List<ExamPhaseDto> Phases { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}