namespace Moeen.Shared.Responses.OwnerDashboard
{
    public class OwnerDashboardResponse
    {
        public int TotalMosques { get; set; }
        public int TotalSupervisors { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalHalqas { get; set; }
        public int TotalOpenComplaints { get; set; }
        public int TotalPosts { get; set; }
        public int TotalBooks { get; set; }
        public List<OwnerMosqueSummaryDto> MosqueSummaries { get; set; } = new();
        public List<OwnerActivityItemDto> RecentActivities { get; set; } = new();
        public List<OwnerAlertDto> Alerts { get; set; } = new();
    }

    public class OwnerMosqueSummaryDto
    {
        public Guid MosqueId { get; set; }
        public string MosqueName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Supervisors { get; set; }
        public int Teachers { get; set; }
        public int Students { get; set; }
        public int Halqas { get; set; }
        public int OpenComplaints { get; set; }
    }

    public class OwnerActivityItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "bi bi-info-circle";
        public DateTime CreatedAt { get; set; }
    }

    public class OwnerAlertDto
    {
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string Severity { get; set; } = "info";
    }
}
