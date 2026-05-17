using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Exam_Halqa
{
    public class AssignHalqaToExamTeacherRequest
    {
        public Guid HalqaId;

        [Required(ErrorMessage = "معرف أستاذ الاختبارات مطلوب")]
        public Guid ExamTeacherId { get; set; } // ✅ تم التعديل من int إلى Guid

        // ✅ تم التعديل من int إلى List<Guid>
        // عشان تقدر تحط فيه حلقة وحدة أو كل الحلقات (IDs)
        [Required(ErrorMessage = "يجب تحديد حلقة واحدة على الأقل")]
        public List<Guid> HalqaIds { get; set; } = new List<Guid>();
        [Required]
        public Guid ExamTypeId { get; set; }
        public Guid FoujId { get; set; }

    }
}