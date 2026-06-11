using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Core.Entities
{
    public class Exam
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
        public int JuzForm { get; set; }

        [Required]
        public int JuzTo { get; set; }

        [Required]
        public int Mark { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
