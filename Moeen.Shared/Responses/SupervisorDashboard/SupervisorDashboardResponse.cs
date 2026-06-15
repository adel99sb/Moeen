namespace Moeen.Shared.Responses.SupervisorDashboard
{
    public class SupervisorDashboardResponse
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalHalqas { get; set; }
        public int OpenAlertsAndComplaints { get; set; }
        public List<HalqaPerformanceDto> HalqaPerformance { get; set; } = new();
        public List<DashboardStudentDto> ExcellentStudents { get; set; } = new();
        public List<DashboardStudentDto> StrugglingStudents { get; set; } = new();
    }

    public class HalqaPerformanceDto
    {
        public string HalqaName { get; set; } = string.Empty;
        public int Tests { get; set; }
        public int Recitations { get; set; }
        public int Attendance { get; set; }
    }

    public class DashboardStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Badge { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
