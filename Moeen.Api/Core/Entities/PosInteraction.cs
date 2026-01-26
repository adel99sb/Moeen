namespace Moeen.Api.Core.Entities
{
    public class PosInteraction 
    {
        public Guid Id {  get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public DateTime date { get; set; }
        public Post Post { get; set; }
        public User User { get; set; }

    }
}
