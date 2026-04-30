using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Enrollment
{
    public class UpdateMemberStatusRequest
    {
        [Required(ErrorMessage = "Member ID is required")]
        public string MemberId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(0, 2, ErrorMessage = "Status must be between 0 and 2")]
        public int Status { get; set; } // 0=Active,1=Inactive,2=Graduated (or suspended)
    }
}