using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.LessonManagement
{
    public class CreateLessonRequest
    {
        [Required(ErrorMessage = "Lesson data is required")]
        public RequsteLessonDto LessonData { get; set; }
    }
}