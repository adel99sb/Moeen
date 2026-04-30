using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamPhase
{
    public class DeleteExamPhaseRequest
    {
        [Required(ErrorMessage = "Phase ID is required")]
        public Guid PhaseId { get; set; }
    }
}