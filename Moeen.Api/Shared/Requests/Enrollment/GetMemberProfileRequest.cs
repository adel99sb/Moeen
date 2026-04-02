using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Enrollment
{
    public class GetMemberProfileRequest
    {
        [Required(ErrorMessage = "Member ID is required")]
        public string MemberId { get; set; }
    }
}