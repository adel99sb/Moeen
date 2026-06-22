using System;

namespace Moeen.Shared.Responses.ExamPhase
{
    public class ExamPhaseDto
    {
        public Guid Id { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public int StartJuz { get; set; }
        public int EndJuz { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}