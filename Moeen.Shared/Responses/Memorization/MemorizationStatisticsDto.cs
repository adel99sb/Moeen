namespace Moeen.Shared.Responses.Memorization
{
    public class MemorizationStatisticsDto
    {
        public int TotalPagesMemorized { get; set; }
        public int TotalJuzCompleted { get; set; }
        public double AveragePagesPerWeek { get; set; }
        public double MasteryRate { get; set; }
    }
}