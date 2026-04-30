namespace Moeen.Api.Core.Entities
{
    public class Fouj
    {
        public  Guid Id {  get; set; }
        public string name { get; set; } 
        public DateTime start_time { get; set; }
        public TimeSpan End_time { get; set; }
        public Guid MosqueId { get; set; } 
        public Mosque Mosque { get; set; } 
        public ICollection<Halqa> Halqas { get; set; }
        public ICollection<ExamTeacherHalqa> ExamTeacherHalqas { get; set; }



    }
}
