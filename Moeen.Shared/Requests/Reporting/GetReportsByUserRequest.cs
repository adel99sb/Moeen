using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Reporting
{
    public class GetReportsByUserRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        public string? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}