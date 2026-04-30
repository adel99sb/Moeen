using Moeen.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Review
{
    public class JuzReviewInputDto
    {
        [Required(ErrorMessage = "Juz number is required")]
        [Range(1, 30, ErrorMessage = "Juz number must be between 1 and 30")]
        public int JuzNumber { get; set; }

        [Required(ErrorMessage = "Overall grade is required")]
        public Grade OverallGrade { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}