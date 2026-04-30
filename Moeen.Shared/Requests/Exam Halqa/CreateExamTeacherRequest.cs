using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Exam_Halqa
{
    public class CreateExamTeacherRequest
    {
        [Required(ErrorMessage = "Exam teacher data is required")]
        public ExamTeacherData ExamTeacherData { get; set; }
    }
}