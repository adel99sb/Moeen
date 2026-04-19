using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ExamQuery
{
    public class GetExamsByPhaseRequest
    {
        [Required(ErrorMessage = "Phase ID is required")]
        public Guid PhaseId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}