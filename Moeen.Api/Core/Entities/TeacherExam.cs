namespace Moeen.Api.Core.Entities
{
    public class TeacherExam
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int ExamId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
