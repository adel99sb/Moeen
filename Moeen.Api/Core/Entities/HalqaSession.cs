namespace Moeen.Api.Core.Entities
{
    public class HalqaSession
    {
        public Guid Id { get; set; }
        public Guid HalqaId { get; set; }
        public DateTime date { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public Halqa Halqa { get; set; } = null!;
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>(); 


    }
}
