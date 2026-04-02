using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Feedback
{
    public class SubmitComplaintRequest
    {
        [Required(ErrorMessage = "Complaint data is required")]
        public ComplaintDto ComplaintData { get; set; }
    }
}