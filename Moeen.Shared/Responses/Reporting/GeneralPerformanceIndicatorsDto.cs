using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Reporting
{
    public class GeneralPerformanceIndicatorsDto
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCircles { get; set; }
        public int TotalComplaints { get; set; }
        public List<StudentScoreDto> TopStudents { get; set; } = [];
        public List<StudentScoreDto> LowStudents { get; set; } = [];
    }

    public class StudentScoreDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}