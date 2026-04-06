
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Authorization
{
    public class RoleDto
    {
        // Id موجود فقط عند التحديث (اختياري)
        public string Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
        public string Name { get; set; }

        // قائمة الصلاحيات (مثل "users.view", "roles.manage", ...)
        public List<string> Permissions { get; set; }
    }
}