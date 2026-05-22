namespace Moeen.Shared.Responses.ExamQuery
{
    public class ExamStatisticsDto
    {
        public int TotalExams { get; set; }
        public double AverageScore { get; set; }
        public double HighestScore { get; set; }   // ÃÖÝ
        public double LowestScore { get; set; }    // ÃÖÝ
        public int PassCount { get; set; }         // ÈÏáÇð ãä PassedCount
        public double PassRate { get; set; }       // ÈÏáÇð ãä SuccessRate

        // íãßäß ÇáÇÍÊÝÇÙ ÈÇáÎÕÇÆÕ ÇáÞÏíãÉ ááÊæÇÝÞ
        public double SuccessRate { get => PassRate; set => PassRate = value; }
        public int PassedCount { get => PassCount; set => PassCount = value; }
        public int FailedCount { get; set; }
    }
}