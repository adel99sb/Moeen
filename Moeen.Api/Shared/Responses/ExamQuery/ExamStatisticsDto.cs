namespace Moeen.Api.Shared.Responses.ExamQuery
{
    public class ExamStatisticsDto
    {
        public int TotalExams { get; set; }
        public double AverageScore { get; set; }
        public double SuccessRate { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
    }
}