using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class MosqueUser
    {
        public Guid MosqueId { get; set; }

        [ForeignKey("MosqueId")]
        public Mosque Mosque { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
