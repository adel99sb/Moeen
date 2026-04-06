using Microsoft.AspNetCore.Identity;

namespace Moeen.Api.Core.Entities
{
    public class User: IdentityUser<Guid>
    {
        //public Guid Id {  get; set; }
        //public string password { get; set; }
        //public int phone {  get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string? gender { get; set; }
        public int font_size { get; set; }
        public int role { get; set; }
        public string? theme { get; set; }
        public string? profile_imageUrl { get; set; }
        public  DateTime created_at {  get; set; }
        public DateTime joinef_at { get; set; }
        public ICollection<Complaint> complaints { get; set; }
        public ICollection<PosInteraction> PosInteractions { get; set; }
    }
}
