namespace Moeen.Api.Core.Entities
{
    public class HalqeSession
    {
        public Guid id { get; set; }
        public Guid HalqaId { get; set; }
        public DateTime date { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public Halqa Halqa { get; set; }
        public ICollection<Attendance> attendances { get; set; } 


    }
}
