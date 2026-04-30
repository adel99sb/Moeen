using System;

namespace Moeen.Shared.Responses.SystemConfiguration
{
    public class SystemLogDto
    {
        public Guid Id { get; set; }
        public string Level { get; set; } // "Info", "Warning", "Error"
        public string Message { get; set; }
        public string Source { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Exception { get; set; }
    }
}