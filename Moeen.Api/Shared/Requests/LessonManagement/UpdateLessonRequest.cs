using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.LessonManagement
{
    public class UpdateLessonRequest
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Lesson data is required")]
        public RequsteLessonDto LessonData { get; set; }
    }
}