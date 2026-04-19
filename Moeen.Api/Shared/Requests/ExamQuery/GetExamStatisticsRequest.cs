using System;

namespace Moeen.Api.Shared.Requests.ExamQuery
{
    public class GetExamStatisticsRequest
    {
        public Guid? CircleId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? StudentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}