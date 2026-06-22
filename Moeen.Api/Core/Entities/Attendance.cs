using Moeen.Shared.Constants;

namespace Moeen.Api.Core.Entities
{
    public class Attendance
    {
        public Guid Id { get; set; }
        public Guid SaturdayLessonId { get; set; }
        public Guid StudentId { get; set; }
        public Guid HalqeSessionId { get; set; }
        public Guid TeacherId { get; set; }

        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }

        public SaturdayLesson SaturdayLesson { get; set; } = null!;
        public Teacher Teacher { get; set; } = null!;
        public HalqaSession HalqeSession { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }
}
