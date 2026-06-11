namespace Moeen.Shared.Requests.Exam
{
    public class CreateExamRequest
    {
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public int JuzForm { get; set; }
        public int JuzTo { get; set; }
        public int Mark { get; set; }
        public DateTime? Date { get; set; }
    }
}