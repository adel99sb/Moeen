using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Memorization
{
    public class GetMemorizationRecordRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Page number is required")]
        [Range(1, 604)]
        public int PageNumber { get; set; }
    }
}