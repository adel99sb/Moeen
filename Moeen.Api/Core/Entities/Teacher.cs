using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class Teacher : User
    {
        public Guid MosqueId { get; set; }
        public Mosque Mosque { get; set; } = null!;

        [Column("boi")]
        public string Bio { get; set; } = string.Empty;

        public string assigned_at { get; set; } = string.Empty;
        public ICollection<Halqa> halaqas { get; set; } = new List<Halqa>();
        public ICollection<ProgressEntry> ProgressEntrys { get; set; } = new List<ProgressEntry>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public DateTime? DateOfBirth { get; set; }
    }
}
