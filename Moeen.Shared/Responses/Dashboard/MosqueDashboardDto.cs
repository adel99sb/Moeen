namespace Moeen.Shared.Responses.Dashboard
{
    public class MosqueDashboardDto
    {
        public Guid MosqueId { get; set; }
        public string MosqueName { get; set; }
        public int TotalStudentsInMosque { get; set; }
        public int TodayPresentStudents { get; set; }
        public int TodayAbsentStudents { get; set; }
        public int TotalPostsCount { get; set; }
    }
}