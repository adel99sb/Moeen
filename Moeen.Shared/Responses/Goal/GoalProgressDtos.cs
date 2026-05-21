using Moeen.Shared.Constants;
using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Goal
{
    public class DailyEntryResultDto
    {
        public Guid StudentId { get; set; }
        public DateTime Date { get; set; }
        public Guid? AttendanceId { get; set; }
        public Guid? MemorizationEntryId { get; set; }
        public Guid? ReviewEntryId { get; set; }
        public Guid? TomorrowReviewEntryId { get; set; }
        public Guid? ExamId { get; set; }
        public int ExtraPoints { get; set; }
    }

    public class StudentProgressSummaryDto
    {
        public Guid StudentId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Points { get; set; }
        public int ExamsCount { get; set; }
        public int ReviewCount { get; set; }
        public int MemorizationCount { get; set; }
        public int SessionsCount { get; set; }
    }

    public class StudentProgressRecordDto
    {
        public Guid RecordId { get; set; }
        public ProgressRecordType RecordType { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;

        public int? JuzNumber { get; set; }
        public int? JuzFrom { get; set; }
        public int? JuzTo { get; set; }
        public int? FromPage { get; set; }
        public int? ToPage { get; set; }

        public int? Score { get; set; }
        public int Points { get; set; }
        public string? GradeLabel { get; set; }
        public string? Notes { get; set; }
    }

    public class HalqaPerformanceOverviewDto
    {
        public Guid? HalqaId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<HalqaPerformancePointDto> Chart { get; set; } = new();
        public List<StudentProgressRankDto> TopStudents { get; set; } = new();
        public List<StudentProgressRankDto> LaggingStudents { get; set; } = new();
    }

    public class HalqaPerformancePointDto
    {
        public DateTime Date { get; set; }
        public int AttendanceCount { get; set; }
        public int MemorizationCount { get; set; }
        public int ExamsCount { get; set; }
    }

    public class StudentProgressRankDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Points { get; set; }
    }
}