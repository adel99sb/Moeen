using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamPhase
{
    public class DefineExamPhaseRequest
    {
        [Required(ErrorMessage = "Phase name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Phase name must be between 2 and 100 characters")]
        public string PhaseName { get; set; }

        [Required(ErrorMessage = "Start Juz is required")]
        [Range(1, 30, ErrorMessage = "Start Juz must be between 1 and 30")]
        public int StartJuz { get; set; }

        [Required(ErrorMessage = "End Juz is required")]
        [Range(1, 30, ErrorMessage = "End Juz must be between 1 and 30")]
        public int EndJuz { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
    }
}