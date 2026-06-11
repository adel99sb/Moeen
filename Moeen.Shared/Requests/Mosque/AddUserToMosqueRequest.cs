namespace Moeen.Shared.Requests.Mosque
{
    public class AddUserToMosqueRequest
    {
        public Guid MosqueId { get; set; }
        public Guid UserId { get; set; }
    }
}
