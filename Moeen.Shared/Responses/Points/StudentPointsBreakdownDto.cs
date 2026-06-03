using System;

namespace Moeen.Shared.Responses.Points
{
    public class StudentPointsBreakdownDto
    {
        public Guid StudentId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalPoints { get; set; }
        public int ProgressPoints { get; set; }
        public int ExamPoints { get; set; }
        public int AttendanceCount { get; set; }
        public int OtherPoints { get; set; }
    }
}