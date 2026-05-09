using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Exam_Halqa
{
    public class GetExamTeacherByIdRequest
    {
        [Required(ErrorMessage = "معرف معلم الامتحان مطلوب")]
        public Guid Id { get; set; }
    }
}