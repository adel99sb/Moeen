using System;

namespace Moeen.Api.Shared.Responses.ExamPhase
{
    public class ExamPhaseDto
    {
        public Guid Id { get; set; }
        public string PhaseName { get; set; }
        public int StartJuz { get; set; }
        public int EndJuz { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}