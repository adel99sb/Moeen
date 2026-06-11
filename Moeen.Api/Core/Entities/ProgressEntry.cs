using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Core.Entities
{
    public class ProgressEntry
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid StudentId { get; set; }
        public User Student { get; set; }

        [Required]
        public Guid TeacherId { get; set; }
        public User Teacher { get; set; }

        [Required]
        public int JuzNumber { get; set; }

        [Required]
        public int PageNumber { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
