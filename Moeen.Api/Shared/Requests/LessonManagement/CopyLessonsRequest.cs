using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.LessonManagement
{
    public class CopyLessonsRequest
    {
        [Required(ErrorMessage = "Source circle ID is required")]
        public Guid SourceCircleId { get; set; }

        [Required(ErrorMessage = "Target circle ID is required")]
        public Guid TargetCircleId { get; set; }
    }
}