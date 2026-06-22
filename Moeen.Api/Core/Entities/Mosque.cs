namespace Moeen.Api.Core.Entities
{
     public class Mosque
    {
        public Guid Id  { get; set; }
        public string name{ get; set; } = string.Empty;
         public string address { get; set; } = string.Empty;
        public string contact_phone { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public ICollection<Fouj> foujs { get; set; } = new List<Fouj>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<TeacherExam> TeacherExams { get; set; } = new List<TeacherExam>();

        public ICollection<PdfFile> pdfFiles { get; set; } = new List<PdfFile>();
        public ICollection<SaturdayLesson> SaturdayLessons { get; set; } = new List<SaturdayLesson>();
        public ICollection<Post> posts { get; set; } = new List<Post>();
        public ICollection<Supervisor> supervisors { get; set; } = new List<Supervisor>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
