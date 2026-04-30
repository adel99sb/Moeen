using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Identity
{
    public class UpdateProfileRequest
    {
        // يمكن إرسال UserId في الـ Request أو استخلاصه من التوكن. للتوحيد نضيفه هنا.
        public string UserId { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be Male or Female")]
        public string Gender { get; set; }

        [Range(8, 72, ErrorMessage = "Font size must be between 8 and 72")]
        public int? FontSize { get; set; }

        public string Theme { get; set; }

        [Url(ErrorMessage = "Invalid image URL")]
        public string ProfileImageUrl { get; set; }
    }
}
