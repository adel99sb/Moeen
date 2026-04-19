using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Authorization;
using Moeen.Api.Shared.Responses.Analytics;
using Moeen.Api.Shared.Responses.Authorization;
using System.Security.Claims;

namespace Moeen.Api.Application.Services
{
    //تعديل
    public class AuthorizationService : IAuthorizationService
    {
        public Task<bool> AssignRoleToUserAsync(AssignRoleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CheckAccessResponse> CheckAccessAsync(CheckAccessRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRoleAsync(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PermissionDto>> GetAllPermissionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<RoleDto>> GetAllRolesAsync(RoleFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task<RoleDto> GetRoleByIdAsync(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PermissionDto>> GetUserPermissionsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserRolesResponse> GetUserRolesAsync(UserRolesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RoleDto> ManageRoleAsync(ManageRoleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveRoleFromUserAsync(RemoveRoleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RoleDto> UpdateRolePermissionsAsync(Guid roleId, UpdatePermissionsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
