using Moeen.Api.Core.Constants;
using System;

namespace Moeen.Api.Shared.Responses.Memorization
{
    public class MemorizationRecordDto
    {
        public Guid RecordId { get; set; }
        public Guid StudentId { get; set; }
        public int PageNumber { get; set; }
        public Grade Grade { get; set; }
        public string? Notes { get; set; }
        public DateTime MemorizedAt { get; set; }
    }
}