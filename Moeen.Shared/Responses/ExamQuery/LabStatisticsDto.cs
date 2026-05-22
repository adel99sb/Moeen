namespace Moeen.Shared.Responses.ExamQuery
{
    public class LabStatisticsDto
    {
        public int TotalExamsConducted { get; set; }
        public int TotalStudentsTested { get; set; }
        public double AverageGrade { get; set; }
        public double PassRate { get; set; }
        public int TotalPointsAwarded { get; set; }
        public string? TopPerformingStudent { get; set; }
        public string? LowestPerformingStudent { get; set; }
    }
}