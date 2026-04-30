using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Review
{
    public class GetReviewsByTeacherRequest
    {
        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}