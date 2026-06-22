namespace Moeen.Api.Core.Entities
{
    public class SaturdayLesson
    {
        public Guid Id { get; set; }
        public Guid SaturdayHalqeId { get; set; }
        public Guid? WeeklyLessonId { get; set; }
        public WeeklyLesson WeeklyLesson { get; set; } = null!;
        public Guid? HalqaId { get; set; }
        public Halqa Halqa { get; set; } = null!;
        public Guid? TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
        public int lesson_number { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public SaturdayHalqa SaturdayHalqe { get; set; } = null!;
        public ICollection<Attendance> attendances { get; set; } = new List<Attendance>();
        public ICollection<PdfFile> PdfFiles { get; set; } = new List<PdfFile>();
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
