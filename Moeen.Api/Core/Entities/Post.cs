using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid MosqueId { get; set; }

        [ForeignKey("MosqueId")]
        public Mosque Mosque { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
