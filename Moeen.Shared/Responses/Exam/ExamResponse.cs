namespace Moeen.Shared.Responses.Exam
{
    public class ExamResponse
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentFullName { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherFullName { get; set; }
        public int JuzForm { get; set; }
        public int JuzTo { get; set; }
        public int Mark { get; set; }
        public DateTime Date { get; set; }
    }
}