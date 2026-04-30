using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamPhase
{
    public class UpdateExamPhaseInfoRequest
    {
        [Required(ErrorMessage = "Phase ID is required")]
        public Guid PhaseId { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Phase name must be between 2 and 100 characters")]
        public string? PhaseName { get; set; }

        [Range(1, 30, ErrorMessage = "Start Juz must be between 1 and 30")]
        public int? StartJuz { get; set; }

        [Range(1, 30, ErrorMessage = "End Juz must be between 1 and 30")]
        public int? EndJuz { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// „” ÊÏ «·„—Õ·… («Œ Ì«—Ì)
        /// </summary>
        [StringLength(50, ErrorMessage = "Level cannot exceed 50 characters")]
        public string? Level { get; set; }
    }
}