using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Enrollment
{
    public class UpdateTeacherInfoRequest
    {
        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        public string? Bio { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Specialty { get; set; }
        public List<string>? Experiences { get; set; }      // ãËáÇğ: "ãÚáã ŞÑÂä ãäĞ 10 ÓäæÇÊ"
        public List<string>? Achievements { get; set; }    // ãËáÇğ: "ÊÎÑíÌ 50 ØÇáÈÇğ"
        public List<string>? TeachingPlaces { get; set; }  // ãËáÇğ: "ÌÇãÚ ÇáßÈíÑ"
    }
}