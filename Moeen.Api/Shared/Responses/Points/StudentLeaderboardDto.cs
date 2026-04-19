using System;

namespace Moeen.Api.Shared.Responses.Points
{
    public class StudentLeaderboardDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int Rank { get; set; }
    }
}