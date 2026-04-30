using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Feedback
{
    public class SubmitComplaintRequest
    {
        [Required(ErrorMessage = "Complaint data is required")]
        public ComplaintDto ComplaintData { get; set; }
    }
}