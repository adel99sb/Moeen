using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.LessonManagement
{
    public class GetLessonMaterialsRequest
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public Guid LessonId { get; set; }
    }
}