using Moeen.Shared.Constants;
using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Mobile
{
    public class StudentProgressResponse
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentInitials { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public DateTime? RegisteredSince { get; set; }
        public StudentProgressStatsDto Stats { get; set; } = new();
        public List<StudentProgressRecordDto> Records { get; set; } = new();
        public StudentProgressSummaryDto Summary { get; set; } = new();
    }

    public class StudentProgressStatsDto
    {
        public int MemorizationSessions { get; set; }
        public int ReviewSessions { get; set; }
        public int ExamsCount { get; set; }
        public int TotalPoints { get; set; }
    }

    public class StudentProgressRecordDto
    {
        public Guid Id { get; set; }
        public ProgressRecordType Type { get; set; }
        public string TypeText { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int? JuzNumber { get; set; }
        public int? JuzFrom { get; set; }
        public int? JuzTo { get; set; }
        public int? FromPage { get; set; }
        public int? ToPage { get; set; }
        public int? Mark { get; set; }
        public int Points { get; set; }
        public string RatingText { get; set; } = string.Empty;
        public string StatusKind { get; set; } = "info";
        public string TeacherNote { get; set; } = string.Empty;
    }

    public class StudentProgressSummaryDto
    {
        public string PeriodTitle { get; set; } = string.Empty;
        public int MemorizationCompletionPercentage { get; set; }
        public int ReviewCompletionPercentage { get; set; }
        public string AverageRatingText { get; set; } = string.Empty;
        public int AverageRatingPercentage { get; set; }
        public int PointsInPeriod { get; set; }
        public int PointsInPeriodPercentage { get; set; }
    }
}
