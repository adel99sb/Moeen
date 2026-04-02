using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.LessonManagement
{
    public class LessonDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public string Content { get; set; } // محتوى الدرس (نص، URL، إلخ)

        public int? Order { get; set; } // ترتيب الدرس ضمن الوحدة

        public Guid? HalqaId { get; set; } // الحلقة المرتبطة

        public TimeSpan? Duration { get; set; } // مدة الدرس
    }
}