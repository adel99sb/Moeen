namespace Moeen.Shared.Responses.TeacherDashboard
{
    public class TeacherDashboardOverviewResponse
    {
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string MosqueName { get; set; } = string.Empty;
        public int TotalHalaqas { get; set; }
        public int TotalStudents { get; set; }
        public int AttendanceTodayPercent { get; set; }
        public int TotalMemorizedPages { get; set; }
        public int ExcellencePoints { get; set; }
        public int FollowUpAlerts { get; set; }
        public List<TeacherDashboardHalaqaMetricDto> Halaqas { get; set; } = new();
        public List<TeacherDashboardStudentProgressDto> TopStudents { get; set; } = new();
        public List<TeacherDashboardStudentAlertDto> FollowUpStudents { get; set; } = new();
    }

    public class TeacherDashboardHalaqaMetricDto
    {
        public Guid HalqaId { get; set; }
        public string HalqaName { get; set; } = string.Empty;
        public string HalqaType { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
        public int ProgressEntriesThisMonth { get; set; }
        public int AttendanceRatePercent { get; set; }
        public int AverageLevelScore { get; set; }
    }

    public class TeacherDashboardStudentProgressDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string HalqaName { get; set; } = string.Empty;
        public int MemorizedUntil { get; set; }
        public int LatestPageNumber { get; set; }
        public int LevelScore { get; set; }
        public int Points { get; set; }
        public DateTime? LastProgressDate { get; set; }
    }

    public class TeacherDashboardStudentAlertDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string HalqaName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TeacherHalaqaProgressResponse
    {
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int TotalHalaqas { get; set; }
        public int TotalStudents { get; set; }
        public int TotalProgressEntries { get; set; }
        public int AverageAttendanceRatePercent { get; set; }
        public List<TeacherHalaqaProgressDto> Halaqas { get; set; } = new();
    }

    public class TeacherHalaqaProgressDto
    {
        public Guid HalqaId { get; set; }
        public string HalqaName { get; set; } = string.Empty;
        public string HalqaType { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
        public int ProgressEntriesCount { get; set; }
        public int AttendanceRatePercent { get; set; }
        public int AverageLevelScore { get; set; }
        public int AverageMemorizationPercent { get; set; }
        public DateTime? LastProgressDate { get; set; }
        public List<TeacherStudentProgressDto> Students { get; set; } = new();
    }

    public class TeacherStudentProgressDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Points { get; set; }
        public int TotalEntries { get; set; }
        public int LatestJuzNumber { get; set; }
        public int LatestPageNumber { get; set; }
        public int MemorizedUntil { get; set; }
        public int NextTarget { get; set; }
        public int LevelScore { get; set; }
        public int AttendanceRatePercent { get; set; }
        public int MemorizationPercent { get; set; }
        public DateTime? LastProgressDate { get; set; }
    }
}
