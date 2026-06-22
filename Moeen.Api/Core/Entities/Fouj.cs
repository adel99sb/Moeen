namespace Moeen.Api.Core.Entities
{
    public class Fouj
    {
        public  Guid Id {  get; set; }
        public string name { get; set; } = string.Empty; 
        public DateTime start_time { get; set; }
        public TimeSpan End_time { get; set; }
        public Guid MosqueId { get; set; } 
        public Mosque Mosque { get; set; } = null!; 
        public ICollection<Halqa> Halqas { get; set; } = new List<Halqa>();
        public ICollection<ExamTeacherHalqa> ExamTeacherHalqas { get; set; } = new List<ExamTeacherHalqa>();



    }
}
