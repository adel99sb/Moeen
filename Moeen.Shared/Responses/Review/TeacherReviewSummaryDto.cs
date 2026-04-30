using System;

namespace Moeen.Shared.Responses.Review
{
    public class TeacherReviewSummaryDto
    {
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;

        public int TotalReviewsRecorded { get; set; }
        public int StudentsReviewedCount { get; set; }
        public double AverageScore { get; set; }
    }
}