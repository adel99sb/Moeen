using Moeen.Shared.Constants;


//using Moeen.Api.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Memorization
{
    public class PageGradeDto
    {
        [Required(ErrorMessage = "Page number is required")]
        [Range(1, 604, ErrorMessage = "Page number must be between 1 and 604")]
        public int PageNumber { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        public Grade Grade { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}