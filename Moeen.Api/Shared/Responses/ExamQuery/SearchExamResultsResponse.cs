using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.ExamQuery
{
    public class SearchExamResultsResponse
    {
        public List<ExamResultDto> Results { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}