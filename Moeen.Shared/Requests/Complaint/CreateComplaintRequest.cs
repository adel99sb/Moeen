namespace Moeen.Shared.Requests.Complaint
{
    public class CreateComplaintRequest
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}