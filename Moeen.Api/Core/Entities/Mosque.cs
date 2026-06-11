using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Core.Entities
{
    public class Mosque
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Address { get; set; }

        public ICollection<MosqueUser> MosqueUsers { get; set; } = new List<MosqueUser>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
