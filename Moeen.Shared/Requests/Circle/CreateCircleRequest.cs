using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Circle
{
    public class CreateCircleRequest
    {
        [Required(ErrorMessage = "Circle name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Circle name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fouj ID is required")]
        public Guid FoujId { get; set; }

        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; } // ملاحظة: في الكلاس الأصلي teacherId كان int، لكنه خطأ، يجب أن يكون Guid

        [Required(ErrorMessage = "Circle type is required")]
        [StringLength(50, ErrorMessage = "Circle type cannot exceed 50 characters")]
        public string Type { get; set; }
    }
}