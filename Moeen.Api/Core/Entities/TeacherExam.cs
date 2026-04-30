namespace Moeen.Api.Core.Entities
{
    public class TeacherExam : User
    {
        public Guid Id { get; set; }
        public Guid MosquId { get; set; }
        public string? Bio { get; set; }
        public Mosque Mosque { get; set; }

        public ICollection<ExamTeacherHalqa> ExamTeacherHalqas { get; set; }
    }
}

