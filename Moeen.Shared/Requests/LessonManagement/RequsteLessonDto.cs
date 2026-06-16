using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.LessonManagement
{
    public class RequsteLessonDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public string Content { get; set; }

        public int? Order { get; set; }

        public Guid? HalqaId { get; set; }

        public TimeSpan? Duration { get; set; }
    }

    public class CreateWeeklyLessonRequest
    {
        [Required(ErrorMessage = "عنوان الدرس الأسبوعي مطلوب")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "عنوان الدرس يجب أن يكون بين 2 و 120 حرف")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "الوصف لا يجب أن يتجاوز 500 حرف")]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateWeeklyLessonRequest : CreateWeeklyLessonRequest
    {
        public Guid Id { get; set; }
    }

    public class CreateWeeklyLessonAssignmentRequest
    {
        public Guid WeeklyLessonId { get; set; }
        public Guid TeacherId { get; set; }
        public Guid HalqaId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class UpdateWeeklyLessonAssignmentRequest : CreateWeeklyLessonAssignmentRequest
    {
        public Guid Id { get; set; }
    }
}
