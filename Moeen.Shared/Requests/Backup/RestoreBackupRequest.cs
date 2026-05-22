using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Backup
{
    public class RestoreBackupRequest
    {
        [Required(ErrorMessage = "«”„ «·„·› „ÿ·Ê»")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "„Õ ÊÏ «·‰”Œ… «·«Õ Ì«ÿÌ… „ÿ·Ê»")]
        public byte[] Content { get; set; }
    }
}