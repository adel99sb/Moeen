using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.LessonManagement
{
    public class ReorderLessonsRequest
    {
        [Required(ErrorMessage = "Module ID is required")]
        public Guid ModuleId { get; set; }

        [Required(ErrorMessage = "Lesson IDs list is required")]
        [MinLength(1, ErrorMessage = "At least one lesson ID is required")]
        public List<Guid> LessonIds { get; set; } = new List<Guid>();
    }
}