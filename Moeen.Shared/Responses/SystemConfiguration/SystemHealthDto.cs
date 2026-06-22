namespace Moeen.Shared.Responses.SystemConfiguration
{
    public class SystemHealthDto
    {
        public string Status { get; set; } = string.Empty; // "Healthy", "Degraded", "Unhealthy"
        public double Uptime { get; set; } // hours
        public long MemoryUsage { get; set; } // bytes
        public double CpuUsage { get; set; } // percentage
        public int ActiveUsers { get; set; }
        public int RecentErrors { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}