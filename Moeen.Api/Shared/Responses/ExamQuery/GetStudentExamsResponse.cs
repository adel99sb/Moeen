using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.ExamQuery
{
    public class GetStudentExamsResponse
    {
        public List<ExamResultDto> Exams { get; set; }
        public int TotalCount { get; set; }
    }
}