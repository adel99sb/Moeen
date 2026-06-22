using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class Student : User
    {
        public Guid SaturdayHalqeId { get; set; }
        public Guid MosqueId { get; set; }
        public Halqa? Halqa { get; set; }
        public int age { get; set; }
        public new string gender { get; set; } = string.Empty;

        [Column("enrollmrnt_date")]
        public DateTime EnrollmentDate { get; set; }

        public int status { get; set; }
        public int score { get; set; }
        public Mosque Mosque { get; set; } = null!;
        public SaturdayHalqa SaturdayHalqa { get; set; } = null!;
        public ICollection<ProgressEntry> progressEntrys { get; set; } = new List<ProgressEntry>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public Guid? ParentId { get; set; }

        public Student Parent { get; set; } = null!;
        public ICollection<Student> Children { get; set; } = new List<Student>();
        public Guid? SaturdayHalqaId { get; internal set; }
        public Guid? HalqaId { get; set; }
    }
}
