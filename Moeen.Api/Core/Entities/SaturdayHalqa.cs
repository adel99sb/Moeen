namespace Moeen.Api.Core.Entities
{
    public class SaturdayHalqa
    {
        public Guid Id { get; set; }        
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
        public string name { get; set; } = string.Empty;
        public int age_min { get; set; }
        public int age_max { get; set; }
        public ICollection<SaturdayLesson> SaturdayLessons { get; set; } = new List<SaturdayLesson>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public Guid MosqueId { get; internal set; }
    }
}
