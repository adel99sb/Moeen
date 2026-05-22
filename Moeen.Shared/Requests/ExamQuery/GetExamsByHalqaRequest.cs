using System;

namespace Moeen.Shared.Requests.ExamQuery
{
    public class GetExamsByHalqaRequest
    {
        public Guid HalqaId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}