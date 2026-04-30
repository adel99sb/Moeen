using System;

namespace Moeen.Shared.Responses.Review
{
    public class StudentReviewSummaryDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public int TotalReviews { get; set; }
        public double AverageScore { get; set; }
        public DateTime? LastReviewDate { get; set; }
    }
}