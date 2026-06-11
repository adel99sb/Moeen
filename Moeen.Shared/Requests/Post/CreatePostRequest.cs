namespace Moeen.Shared.Requests.Post
{
    public class CreatePostRequest
    {
        public Guid MosqueId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
