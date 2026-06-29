using System.ComponentModel.DataAnnotations;

namespace Moeen.Dashboard.Components.Shared
{
    public class MemberManagementDialogModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "الاسم يجب أن يكون بين 2 و 100 حرف")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الهاتف غير صالح")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "الجنس مطلوب")]
        public string Gender { get; set; } = "Male";

        public DateTime JoinedAt { get; set; } = DateTime.Today;

        [Range(0, 2, ErrorMessage = "حالة الحساب غير صالحة")]
        public int Status { get; set; }

        [Range(0, 100, ErrorMessage = "النقاط يجب أن تكون بين 0 و 100")]
        public int Score { get; set; }

        public Guid MosqueId { get; set; }
        public Guid FoujId { get; set; }
        public Guid? HalqaId { get; set; }

        [StringLength(100, ErrorMessage = "كلمة المرور لا يمكن أن تتجاوز 100 حرف")]
        public string? Password { get; set; }

        [StringLength(500, ErrorMessage = "النبذة لا يجب أن تتجاوز 500 حرف")]
        public string? Bio { get; set; }

        public DateTime? AssignedAt { get; set; } = DateTime.Today;
    }
}
