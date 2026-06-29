namespace Moeen.Api.Core.Entities
{
    public class SaturdayLessonStudent
    {
        public Guid SaturdayLessonId { get; set; }
        public SaturdayLesson SaturdayLesson { get; set; } = null!;

        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
