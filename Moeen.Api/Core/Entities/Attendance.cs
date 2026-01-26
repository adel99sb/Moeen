namespace Moeen.Api.Core.Entities
{
    public class Attendance
    {
        public Guid Id {  get; set; }
        public Guid SaturdayLessonId { get; set; }
        public Guid StudentId { get; set; }
        public Guid HalqeSessionId { get; set; }
        public Guid TeacherId { get; set; }
        public SaturdayLesson SaturdayLesson { get; set; }
        public Teacher Teacher { get; set; }
        public HalqaSession HalqeSession { get; set; }
        public Student Student { get; set; }
    }
}
