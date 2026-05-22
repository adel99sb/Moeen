namespace Moeen.Shared.Responses.ExamQuery
{
    public class HalqaPerformanceDto
    {
        public string HalqaName { get; set; }
        public double AverageScore { get; set; }
        public int ExamsCount { get; set; }
        public int StudentsCount { get; set; }
    }
}