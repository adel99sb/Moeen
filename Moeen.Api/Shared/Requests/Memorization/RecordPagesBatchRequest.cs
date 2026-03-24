using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Memorization
{
    public class RecordPagesBatchRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Pages grades list is required")]
        [MinLength(1, ErrorMessage = "At least one page must be provided")]
        public List<PageGradeDto> PagesGrades { get; set; }
    }
}