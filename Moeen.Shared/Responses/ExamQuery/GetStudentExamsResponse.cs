using Moeen.Shared.Responses.ExamCommand;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.ExamQuery
{
    public class GetStudentExamsResponse
    {
        public List<ExamResultDto> Exams { get; set; } = new List<ExamResultDto>();
        public int TotalCount { get; set; }
    }
}