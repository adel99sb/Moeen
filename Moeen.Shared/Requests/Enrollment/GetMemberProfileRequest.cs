using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Enrollment
{
    public class GetMemberProfileRequest
    {
        [Required(ErrorMessage = "Member ID is required")]
        public string MemberId { get; set; }
    }
}