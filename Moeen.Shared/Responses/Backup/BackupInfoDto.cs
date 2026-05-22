using System;

namespace Moeen.Shared.Responses.Backup
{
    public class BackupInfoDto
    {
        public string FileName { get; set; }
        public long SizeBytes { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}