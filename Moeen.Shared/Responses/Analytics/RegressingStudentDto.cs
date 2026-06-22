using System;

namespace Moeen.Shared.Responses.Analytics
{
    public class RegressingStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public double PreviousAveragePages { get; set; }
        public double RecentAveragePages { get; set; }
        public double Delta { get; set; }
    }
}