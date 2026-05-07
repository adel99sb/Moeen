using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string name { get; set; }
        public string? gender { get; set; }
        public int font_size { get; set; }
        public int role { get; set; }
        public string? theme { get; set; }
        public string? profile_imageUrl { get; set; }
        public DateTime created_at { get; set; }

        [Column("joinef_at")]
        public DateTime JoinedAt { get; set; }

        public ICollection<Complaint> complaints { get; set; }
        public ICollection<PosInteraction> PosInteractions { get; set; }
    }
}
