using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Memorization
{
    public class ResetMemorizationRecordRequest
    {
        [Required(ErrorMessage = "Record ID is required")]
        public Guid RecordId { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }
}