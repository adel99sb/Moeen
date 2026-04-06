namespace Moeen.Api.Shared.Requests.Authorization
{
    public class ManageRoleRequest
    {
        // إذا كان null فهذا يعني إنشاء دور جديد، وإذا موجود فهذا تحديث
        public string RoleId { get; set; }

        public RequsteRoleDto RoleData { get; set; }
    }
}