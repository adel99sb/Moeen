using Moeen.Api.Core.Constants;
using System;

namespace Moeen.Api.Shared.Responses.Review
{
    public class ReviewRecordDto
    {
        public Guid ReviewRecordId { get; set; }
        public Guid StudentId { get; set; }

        public int? PageNumber { get; set; }
        public int? JuzNumber { get; set; }

        public ReviewType ReviewType { get; set; }
        public Grade Grade { get; set; }

        public string? Notes { get; set; }
        public DateTime ReviewedAt { get; set; }
    }
}