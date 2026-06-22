using System.Collections.Generic;

namespace Moeen.Shared.Responses.Authorization
{
    public class UserRolesResponse
    {
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    }
}