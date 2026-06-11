using Moeen.Shared.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class Attendance
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        [ForeignKey("StudentId")]
        public User Student { get; set; }

        [Required]
        public Guid MosqueId { get; set; }

        [ForeignKey("MosqueId")]
        public Mosque Mosque { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        [StringLength(250)]
        public string? Note { get; set; }
    }
}
