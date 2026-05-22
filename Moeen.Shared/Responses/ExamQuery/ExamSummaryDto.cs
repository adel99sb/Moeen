using System;

namespace Moeen.Shared.Responses.ExamQuery
{
    public class ExamSummaryDto
    {
        public Guid ExamId { get; set; }
        public string StudentName { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }
}