using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.LessonManagement
{
    public class AddLessonMaterialsRequest
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public Guid LessonId { get; set; }

        [Required(ErrorMessage = "Materials list is required")]
        [MinLength(1, ErrorMessage = "At least one material is required")]
        public List<MaterialDto> Materials { get; set; }
    }
}