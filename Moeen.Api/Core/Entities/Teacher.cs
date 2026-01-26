namespace Moeen.Api.Core.Entities
{
    public class Teacher : User
    {
        public Guid MosqueId { get; set; }
        public Mosque Mosque { get; set; }
        public string boi { get; set; }
        public string assigned_at { get; set; }
        public ICollection<Halqa> halaqas { get; set; }
        public ICollection<ProgressEntry> ProgressEntrys { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
    }
}
