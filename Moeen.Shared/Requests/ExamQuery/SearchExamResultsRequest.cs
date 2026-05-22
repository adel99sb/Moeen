using System;

namespace Moeen.Shared.Requests.ExamQuery
{
    public class SearchExamResultsRequest
    {
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}