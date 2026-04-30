using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamPhase
{
    public class GetExamPhasesByCircleRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}