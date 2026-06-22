using System;

namespace Moeen.Shared.Responses.Backup
{
    public class BackupInfoDto
    {
        public string FileName { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}