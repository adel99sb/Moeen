using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class Student : User
    {
        public Guid SaturdayHalqeId { get; set; }
        public Guid MosqueId { get; set; }
        public int age { get; set; }
        public string gender { get; set; }

        [Column("enrollmrnt_date")]
        public DateTime EnrollmentDate { get; set; }

        public int status { get; set; }
        public int score { get; set; }
        public Mosque Mosque { get; set; }
        public SaturdayHalqa SaturdayHalqa { get; set; }
        public ICollection<ProgressEntry> progressEntrys { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
        public Guid? ParentId { get; set; }

        public Student Parent { get; set; }
        public ICollection<Student> Children { get; set; }
        public Guid? SaturdayHalqaId { get; internal set; }
    }
}
