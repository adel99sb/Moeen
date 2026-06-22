namespace Moeen.Api.Core.Entities
{
    public class Post
    {
        public Guid Id {  get; set; }
        public Guid MosqueId { get; set; }
        public Guid? HalqaId { get; set; }
        public string title { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
        public string imageUrl { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public Mosque Mosque { get; set; } = null!;
        public Halqa? Halqa { get; set; }
        public ICollection<PosInteraction> PosInteractions { get; set; } = new List<PosInteraction>();


    }
}
