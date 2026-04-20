using Moeen.Api.Shared.Responses.ExamCommand;
using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.ExamQuery
{
    public class ExportExamDataDto
    {
        public IReadOnlyList<ExamResultDto> Rows { get; set; } = new List<ExamResultDto>();
        public int TotalCount { get; set; }
    }
}