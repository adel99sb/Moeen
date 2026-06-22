using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Authorization
{
    public class RequsteRoleDto
    {
        // Id موجود فقط عند التحديث (اختياري)
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        // قائمة الصلاحيات (مثل "users.view", "roles.manage", ...)
        public List<string> Permissions { get; set; } = new List<string>();
    }
}