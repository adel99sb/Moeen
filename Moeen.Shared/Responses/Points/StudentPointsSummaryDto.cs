using System;

namespace Moeen.Shared.Responses.Points
{
    public class StudentPointsSummaryDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
    }
}