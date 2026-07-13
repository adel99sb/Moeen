namespace Moeen.Api.Core.Entities
{
    public class Exam
    {
        public Guid Id {  get; set; }
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public int juz_form {  get; set; }
        public int juz_to { get; set; }
        public int score { get; set; }
        public DateTime date { get; set; }

        public string notes { get; set; } = string.Empty;
        public int mark { get; set; }
        public Student Student { get; set; } = null!;
        public Teacher Teacher { get; set; } = null!;
        public Guid? TeacherExamId { get; set; }
        public TeacherExam? TeacherExams { get; set; }

    }
}
