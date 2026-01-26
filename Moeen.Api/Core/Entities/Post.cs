namespace Moeen.Api.Core.Entities
{
    public class Post
    {
        public Guid Id {  get; set; }
        public Guid MosqueId { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string imageUrl { get; set; }
        public DateTime created_at { get; set; }
        public Mosque Mosque { get; set; }
        public ICollection<PosInteraction> PosInteractions { get; set; }


    }
}
