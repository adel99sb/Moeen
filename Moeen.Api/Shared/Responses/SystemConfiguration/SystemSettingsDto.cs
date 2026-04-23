namespace Moeen.Api.Shared.Responses.SystemConfiguration
{
    public class SystemSettingsDto
    {
        public string SystemName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;

        public bool EnableNotifications { get; set; }
        public bool EnableBackup { get; set; }
        public string BackupSchedule { get; set; } = string.Empty;

        public int MaxLoginAttempts { get; set; }
        public int SessionTimeoutMinutes { get; set; }
        public bool MaintenanceMode { get; set; }
    }
}