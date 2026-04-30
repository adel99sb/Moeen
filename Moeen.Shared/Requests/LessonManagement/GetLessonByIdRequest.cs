using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.LessonManagement
{
    public class GetLessonByIdRequest
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public Guid LessonId { get; set; }
    }
}