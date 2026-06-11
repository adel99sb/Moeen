namespace Moeen.Shared.Responses.Post
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public Guid MosqueId { get; set; }
        public string MosqueName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}