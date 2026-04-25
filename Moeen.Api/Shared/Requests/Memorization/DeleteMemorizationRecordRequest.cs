using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Memorization
{
    public class DeleteMemorizationRecordRequest
    {
        [Required(ErrorMessage = "Record ID is required")]
        public Guid RecordId { get; set; }
    }
}