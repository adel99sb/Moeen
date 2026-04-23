using System;
using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Memorization
{
    public class MemorizationProgressReportDto
    {
        public Guid StudentId { get; set; }
        public List<MemorizationProgressPointDto> Points { get; set; } = new();
    }

    public class MemorizationProgressPointDto
    {
        public DateTime Date { get; set; }
        public int PagesMemorizedCumulative { get; set; }
    }
}