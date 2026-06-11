namespace Moeen.Shared.Responses.Dashboard
{
    public class StudentDashboardDto
    {
        public double AttendanceRate { get; set; }
        public int TotalExamsPassed { get; set; }
        public double AverageExamMark { get; set; }
        public int LastMemorizedJuz { get; set; }
        public int LastMemorizedPage { get; set; }
    }
}