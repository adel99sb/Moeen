using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.LessonManagement
{
    public class DeleteLessonRequest
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public Guid Id { get; set; }
    }
}