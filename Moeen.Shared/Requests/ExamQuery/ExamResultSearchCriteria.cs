using System;

namespace Moeen.Shared.Requests.ExamQuery
{
    public class ExamResultSearchCriteria
    {
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? JuzFrom { get; set; }
        public int? JuzTo { get; set; }
        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Date";
        public bool SortDescending { get; set; } = true;
    }
}