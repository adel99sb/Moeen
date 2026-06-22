using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Enrollment
{
    public class CancelMembershipRequest
    {
        [Required(ErrorMessage = "Member ID is required")]
        public string MemberId { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;
    }
}