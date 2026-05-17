using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamQuery
{
    public class CompareHalqasRequest
    {
        [Required(ErrorMessage = "At least two halqa IDs are required")]
        [MinLength(2)]
        public List<Guid> HalqaId { get; set; } = new();

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}