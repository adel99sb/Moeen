namespace Moeen.Api.Shared.Responses.Exam_Halqa
{
    public class ExamTeacherResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int MosqueId { get; set; }
        public string? MosqueName { get; set; }
        public string? Bio { get; set; }
        public DateTime AssignedAt { get; set; }
        public List<int> HalqaIds { get; set; } = new();
    }
}