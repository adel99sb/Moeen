using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Exam_Halqa
{
    public class ExamTeacherData
    {
        public Guid Id; 
        public Guid HalqaId;

        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Mosque ID is required")]
        public Guid MosqueId { get; set; }

        public string? Bio { get; set; }

        [Required(ErrorMessage = "At least one Halqa is required")]
        public List<Guid> HalqaIds { get; set; } = new();
        public Guid TeacherId { get; set; }
        // ✅ أضف ExamTypeId إذا لازم
        public Guid ExamTypeId { get; set; }

        // ✅ أضف AssignedDate إذا لازم
        public DateTime? AssignedDate { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid TeacherExamId { get; set; }
        public Guid MosquId { get; set; }

        // public DateTime? CreatedAt { get; set; }
    }
}
