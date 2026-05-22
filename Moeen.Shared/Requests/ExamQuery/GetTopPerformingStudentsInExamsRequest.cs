using System;

namespace Moeen.Shared.Requests.ExamQuery
{
    public class GetTopPerformingStudentsInExamsRequest
    {
        public int TopCount { get; set; } = 10;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}