using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ExamQuery
{
    public class CompareCirclesRequest
    {
        [Required(ErrorMessage = "At least two circle IDs are required")]
        [MinLength(2)]
        public List<Guid> CircleIds { get; set; } = new();

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}