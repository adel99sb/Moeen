using System;
using Moeen.Api.Core.Enums;

namespace Moeen.Api.Shared.Responses.Points
{
    public class PointsTransactionDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public int Points { get; set; }
        public string TransactionType { get; set; } // "Earned" أو "Spent" أو "Bonus"
        public string Source { get; set; } // مثلاً "PageMemorization", "JuzReview", "Exam"
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
    }
}