using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.ExamQuery
{
    using System;

    namespace Moeen.Shared.Responses.ExamQuery
    {
        public class HalqaExamAnalyticsDto
        {
            public Guid HalqaId { get; set; }
            public string HalqaName { get; set; } = string.Empty;
            public double AverageScore { get; set; }
            public int ExamsCount { get; set; }          // √÷›
            public string TopStudent { get; set; }       // √÷›
            public string LowestStudent { get; set; }    // √÷›
            public double SuccessRate { get; set; }      // «Œ Ì«—Ì
            public List<StudentExamPerformanceDto> TopStudents { get; set; } = new();
            public List<StudentExamPerformanceDto> LowStudents { get; set; } = new();
        }
    }

    public class StudentExamPerformanceDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
}