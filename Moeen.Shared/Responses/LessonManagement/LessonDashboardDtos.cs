
using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.LessonManagement
{
    public class WeeklyLessonDashboardDto
    {
        public DateTime Date { get; set; }
        public List<CircleLessonCardDto> Circles { get; set; } = new();
    }

    public class CircleLessonCardDto
    {
        public Guid CircleId { get; set; }
        public string CircleName { get; set; } = string.Empty;
        public string CurrentLessonTitle { get; set; } = string.Empty;
        public Guid? CurrentLessonId { get; set; }
        public List<LessonStudentStatusDto> Students { get; set; } = new();
    }

    public class LessonStudentStatusDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Points { get; set; }
    }

    public class CircleOverviewDto
    {
        public Guid CircleId { get; set; }
        public string CircleName { get; set; } = string.Empty;
        public string AgeRange { get; set; } = string.Empty;
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int StudentsCount { get; set; }
    }


    public class LessonHistoryItemDto
    {
        public Guid LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
    }

    public class StudentDailyLessonsDto
    {
        public Guid StudentId { get; set; }
        public DateTime Date { get; set; }
        public List<StudentLessonItemDto> Lessons { get; set; } = new();
    }

    public class StudentLessonItemDto
    {
        public Guid LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string CircleName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }


    public class WeeklyLessonManagementDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int AssignmentsCount { get; set; }
        public List<WeeklyLessonAssignmentDto> Assignments { get; set; } = new();
    }

    public class WeeklyLessonAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid WeeklyLessonId { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public Guid HalqaId { get; set; }
        public string HalqaName { get; set; } = string.Empty;
        public string FoujName { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
        public List<Guid> StudentIds { get; set; } = new();
        public string StudentNames { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
