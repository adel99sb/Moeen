namespace Moeen.Api.Shared.Responses.Mosuq
{
    public class MosqueStatisticsDto
    {
        public int CirclesCount { get; set; }
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int Capacity { get; set; }
        public double OccupancyRate { get; set; }
    }
}