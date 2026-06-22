using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Mobile
{
    public class StudentProfileResponse
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentInitials { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public string ParentPhoneNumber { get; set; } = string.Empty;
        public string MosqueName { get; set; } = string.Empty;
        public string FoujName { get; set; } = string.Empty;
        public string HalqaName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public DateTime? EnrollmentDate { get; set; }
        public int TotalPoints { get; set; }
        public List<string> Achievements { get; set; } = new();
        public List<StudentProfileScheduleItemDto> WeeklySchedule { get; set; } = new();
    }

    public class StudentProfileScheduleItemDto
    {
        public string DayName { get; set; } = string.Empty;
        public string LessonTitle { get; set; } = string.Empty;
        public string TimeRange { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public string StatusKind { get; set; } = "upcoming";
    }

    public class SubmitStudentProfileNoteRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}
