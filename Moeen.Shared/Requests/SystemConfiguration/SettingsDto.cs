using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.SystemConfiguration
{
    public class SettingsDto
    {
        [StringLength(200, ErrorMessage = "System name cannot exceed 200 characters")]
        public string SystemName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid contact email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid contact phone")]
        public string ContactPhone { get; set; } = string.Empty;

        public bool EnableNotifications { get; set; }
        public bool EnableBackup { get; set; }
        public string BackupSchedule { get; set; } = string.Empty;
        public int MaxLoginAttempts { get; set; }
        public int SessionTimeoutMinutes { get; set; }
        public bool MaintenanceMode { get; set; }
    }
}