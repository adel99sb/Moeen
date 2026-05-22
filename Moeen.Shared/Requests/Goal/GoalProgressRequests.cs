using Moeen.Shared.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Goal
{
    public class RecordDailyEntryRequest
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public AttendanceStatus? AttendanceStatus { get; set; }
        public string? AttendanceNote { get; set; }

        public MemorizationEntryDto? Memorization { get; set; }
        public ReviewEntryDto? Review { get; set; }
        public RequiredReviewEntryDto? ReviewRequiredForTomorrow { get; set; }
        public ExamEntryDto? Exam { get; set; }

        public int ExtraPoints { get; set; }
        public string? TeacherNotes { get; set; }
    }

    public class UpdateProgressRecordRequest
    {
        [Required]
        public Guid RecordId { get; set; }

        [Required]
        public ProgressRecordType RecordType { get; set; }

        public DateTime? Date { get; set; }
        public int? JuzNumber { get; set; }
        public int? JuzFrom { get; set; }
        public int? JuzTo { get; set; }
        public int? FromPage { get; set; }
        public int? ToPage { get; set; }
        public Grade? Grade { get; set; }
        public int? Score { get; set; }
        public int? Mark { get; set; }
        public string? Notes { get; set; }
        public Guid? TeacherExamId { get; set; }
    }

    public class GetStudentProgressHistoryRequest
    {
        [Required]
        public Guid StudentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ProgressRecordType? RecordType { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetStudentProgressSummaryRequest
    {
        [Required]
        public Guid StudentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetHalqaPerformanceOverviewRequest
    {
        public Guid? HalqaId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class MemorizationEntryDto
    {
        [Range(1, 30)]
        public int JuzNumber { get; set; }

        [Range(1, 604)]
        public int FromPage { get; set; }

        [Range(1, 604)]
        public int ToPage { get; set; }

        public Grade Grade { get; set; }
    }

    public class ReviewEntryDto
    {
        [Range(1, 30)]
        public int JuzNumber { get; set; }

        [Range(1, 604)]
        public int FromPage { get; set; }

        [Range(1, 604)]
        public int ToPage { get; set; }

        public Grade Grade { get; set; }
    }

    public class RequiredReviewEntryDto
    {
        [Range(1, 30)]
        public int JuzNumber { get; set; }

        [Range(1, 604)]
        public int FromPage { get; set; }

        [Range(1, 604)]
        public int ToPage { get; set; }

        public string? Notes { get; set; }
    }

    public class ExamEntryDto
    {
        [Range(1, 30)]
        public int JuzFrom { get; set; }

        [Range(1, 30)]
        public int JuzTo { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        [Range(0, 100)]
        public int Mark { get; set; }

        public string? Notes { get; set; }
        public Guid? TeacherExamId { get; set; }
    }
}