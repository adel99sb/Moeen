using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamPhase
{
    public class GetExamPhaseByIdRequest
    {
        [Required(ErrorMessage = "Phase ID is required")]
        public Guid PhaseId { get; set; }
    }
}