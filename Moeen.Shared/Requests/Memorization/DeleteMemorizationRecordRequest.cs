using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Memorization
{
    public class DeleteMemorizationRecordRequest
    {
        [Required(ErrorMessage = "Record ID is required")]
        public Guid RecordId { get; set; }
    }
}