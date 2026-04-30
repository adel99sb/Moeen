using Moeen.Shared.Constants;


//using Moeen.Api.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Points
{
    public class GradePointDto
    {
        [Required(ErrorMessage = "Grade is required")]
        public Grade Grade { get; set; }

        [Required(ErrorMessage = "Points value is required")]
        [Range(0, 1000, ErrorMessage = "Points must be between 0 and 1000")]
        public int Points { get; set; }
    }
}