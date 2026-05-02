using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.LessonManagement
{
    public class DuplicateLessonRequest
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public Guid LessonId { get; set; }

        public Guid? TargetCircleId { get; set; }

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? NewTitle { get; set; }
    }
}