using System;

namespace Moeen.Api.Shared.Responses.Memorization
{
    public class StudentMemorizationSummaryDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int LastMemorizedPage { get; set; }
        public int TotalPagesMemorized { get; set; }
        public double ProgressPercentage { get; set; }
    }
}