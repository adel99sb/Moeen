using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.LessonManagement
{
    public class ManageLessonTimeRequest
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public Guid LessonId { get; set; }

        [Required(ErrorMessage = "Time configuration is required")]
        public TimeConfig Config { get; set; }
    }
}