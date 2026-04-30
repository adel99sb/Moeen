using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Exam_Halqa
{
    public class ExamTeacherData
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Mosque ID is required")]
        public int MosqueId { get; set; }

        public string? Bio { get; set; }

        [Required(ErrorMessage = "At least one Halqa is required")]
        public List<int> HalqaIds { get; set; } = new();
    }
}