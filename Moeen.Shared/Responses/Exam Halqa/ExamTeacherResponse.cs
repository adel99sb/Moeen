
namespace Moeen.Api.Shared.Responses.Exam_Halqa
{
    public class ExamTeacherResponse
    {
        public Guid Id { get; set; }
        public Guid TeacherExamId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public Guid ExamTeacherHalqaId { get; set; }
        public Guid ExamTypeId { get; set; }  
        public DateTime? AssignedDate { get; set; }
        public bool IsActive { get; set; }
        public List<int> HalqaIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? HalqaName { get; set; }
        public Guid MosqueId { get; set; }
        public string? Bio { get; set; }
        public object HalqasCount { get; set; }
    }
}