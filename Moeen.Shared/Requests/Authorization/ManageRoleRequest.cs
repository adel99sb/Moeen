namespace Moeen.Shared.Requests.Authorization
{
    public class ManageRoleRequest
    {
        // إذا كان null فهذا يعني إنشاء دور جديد، وإذا موجود فهذا تحديث
        public string RoleId { get; set; } = string.Empty;

        public RequsteRoleDto RoleData { get; set; } = null!;
    }
}