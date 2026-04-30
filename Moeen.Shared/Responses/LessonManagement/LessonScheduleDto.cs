using System;

namespace Moeen.Shared.Responses.LessonManagement
{
    public class LessonScheduleDto
    {
        public Guid LessonId { get; set; }
        public Guid CircleId { get; set; }

        public Guid? TeacherId { get; set; }
        public string? TeacherName { get; set; }

        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }

        public string? Location { get; set; }
    }
}