using System;

namespace Moeen.Shared.Responses.Fouj
{
    public class FoujDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid MosqueId { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int HalqasCount { get; set; }
    }
}