using System;

namespace Moeen.Shared.Responses.SystemConfiguration
{
    public class SystemLogDto
    {
        public Guid Id { get; set; }
        public string Level { get; set; } = string.Empty; // "Info", "Warning", "Error"
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Exception { get; set; } = string.Empty;
    }
}