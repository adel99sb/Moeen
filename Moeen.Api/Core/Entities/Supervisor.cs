namespace Moeen.Api.Core.Entities
{
    public class Supervisor : User
    {
        public Guid MosqueId { get; set; }
        public Mosque Mosque { get; set; } = null!;
        public DateTime assigned_at { get; set; }
        
    }
}
