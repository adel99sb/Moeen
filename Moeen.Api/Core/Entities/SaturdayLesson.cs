namespace Moeen.Api.Core.Entities
{
    public class SaturdayLesson
    {
        public Guid Id { get; set; }
        public Guid SaturdayHalqeId { get; set; }
        public Guid? WeeklyLessonId { get; set; }
        public WeeklyLesson WeeklyLesson { get; set; }
        public Guid? HalqaId { get; set; }
        public Halqa Halqa { get; set; }
        public Guid? TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public int lesson_number { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public SaturdayHalqa SaturdayHalqe { get; set; }
        public ICollection<Attendance> attendances { get; set; }
        public ICollection<PdfFile> PdfFiles { get; set; }
    }

    public class WeeklyLesson
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<SaturdayLesson> Schedules { get; set; } = new List<SaturdayLesson>();
    }
}
