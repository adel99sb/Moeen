using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Points
{
    public class SetupPointsSystemRequest
    {
        [Required(ErrorMessage = "Points map is required")]
        [MinLength(1, ErrorMessage = "At least one grade-point mapping is required")]
        public List<GradePointDto> PointsMap { get; set; }
    }
}