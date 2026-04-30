using Moeen.Shared.Responses.ExamCommand;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.ExamQuery
{
    public class ExportExamDataDto
    {
        public IReadOnlyList<ExamResultDto> Rows { get; set; } = new List<ExamResultDto>();
        public int TotalCount { get; set; }
    }
}