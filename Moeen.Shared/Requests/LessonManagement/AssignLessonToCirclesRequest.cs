using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.LessonManagement
{
    public class AssignLessonToCirclesRequest
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public Guid LessonId { get; set; }

        [Required(ErrorMessage = "At least one circle is required")]
        [MinLength(1)]
        public List<Guid> CircleIds { get; set; } = new();
    }
}