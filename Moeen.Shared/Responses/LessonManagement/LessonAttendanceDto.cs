using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.LessonManagement
{
    public class LessonAttendanceDto
    {
        public Guid LessonId { get; set; }
        public DateTime Date { get; set; }

        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }

        public List<LessonAttendanceResultItemDto> Entries { get; set; } = new();
    }

    public class LessonAttendanceResultItemDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}