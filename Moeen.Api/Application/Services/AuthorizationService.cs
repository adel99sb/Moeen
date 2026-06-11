using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.Authorization;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Authorization;

namespace Moeen.Api.Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private const string PermissionClaimType = "permission";
        private const int TeacherRole = 1;
        private const int StudentRole = 2;
        private const int ParentRole = 3;
        private const int SupervisorRole = 4;

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppDbContext _context;

        public AuthorizationService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ICurrentUserService currentUserService,
            AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<GeneralResponse> GetAccountsAsync(AccountFilterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Role))
                return GeneralResponse.BadRequest("Role is required.");

            var roleName = request.Role.Trim();
            if (!await _roleManager.RoleExistsAsync(roleName))
                return GeneralResponse.NotFound("Role not found.");

            // Load IDs of users in role (Identity API does this)
            var roleUsers = await _userManager.GetUsersInRoleAsync(roleName);
            var roleUserIds = roleUsers.Select(u => u.Id).ToList();

            // Build IQueryable and let EF Core execute filters
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim();
                query = query.Where(u =>
                    (u.name ?? string.Empty).Contains(keyword) ||
                    (u.Email ?? string.Empty).Contains(keyword) ||
                    (u.PhoneNumber ?? string.Empty).Contains(keyword));
            }

            if (request.IsActive.HasValue)
            {
                var now = DateTimeOffset.UtcNow;
                // avoid client-eval: translate Active check into LINQ
                query = request.IsActive.Value
                    ? query.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd <= now)
                    : query.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd > now);
            }

            // Filter by role membership using IDs list (roleUserIds is loaded from identity)
            query = query.Where(u => roleUserIds.Contains(u.Id));

            var total = await query.CountAsync();

            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

            var users = await query
                .OrderBy(u => u.name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = await MapAccountsAsync(roleName, users);
            return GeneralResponse.Ok("Accounts retrieved.", result, pageNumber, pageSize, total);
        }

        public async Task<GeneralResponse> GetAccountByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return GeneralResponse.BadRequest("Invalid user id.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            var dto = await MapAccountDetailsAsync(user, role);
            return GeneralResponse.Ok("Account retrieved.", dto);
        }

        public async Task<GeneralResponse> UpsertAccountAsync(UpsertAccountRequest request)
        {
            var auth = await EnsureOwnerAsync();
            if (auth != null) return auth;

            if (request == null || string.IsNullOrWhiteSpace(request.Role))
                return GeneralResponse.BadRequest("Invalid request.");

            var roleName = request.Role.Trim();
            if (!await _roleManager.RoleExistsAsync(roleName))
                return GeneralResponse.NotFound("Role not found.");

            return !request.UserId.HasValue
                ? await CreateAccountAsync(request, roleName)
                : await UpdateAccountAsync(request, roleName);
        }

        public async Task<GeneralResponse> UpdateAccountStatusAsync(UpdateAccountStatusRequest request)
        {
            var auth = await EnsureOwnerAsync();
            if (auth != null) return auth;

            if (request == null || request.UserId == Guid.Empty)
                return GeneralResponse.BadRequest("Invalid request.");

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            await _userManager.SetLockoutEnabledAsync(user, true);
            //var lockoutEnd = request.IsActive ? null : DateTimeOffset.UtcNow.AddYears(100);
            DateTimeOffset? lockoutEnd = request.IsActive ? null : DateTimeOffset.UtcNow.AddYears(100);

            await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            return GeneralResponse.Ok("Account status updated.");
        }

        public async Task<GeneralResponse> DeleteAccountAsync(Guid userId)
        {
            var auth = await EnsureOwnerAsync();
            if (auth != null) return auth;

            if (userId == Guid.Empty)
                return GeneralResponse.BadRequest("Invalid user id.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return GeneralResponse.BadRequest("Failed to delete user.");

            return GeneralResponse.Ok("Account deleted.");
        }

        public async Task<GeneralResponse> AssignRoleToUserAsync(AssignRoleRequest request)
        {
            //var auth = await EnsureOwnerAsync();
            //if (auth != null) return auth;

            if (request == null)
                return GeneralResponse.BadRequest("Invalid request.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
                return GeneralResponse.NotFound("Role not found.");

            if (await _userManager.IsInRoleAsync(user, role.Name))
                return GeneralResponse.Ok("User already in role.");

            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (!result.Succeeded)
                return GeneralResponse.BadRequest("Failed to assign role.");

            return GeneralResponse.Ok("Role assigned.");
        }

        public async Task<GeneralResponse> RemoveRoleFromUserAsync(RemoveRoleRequest request)
        {
            var auth = await EnsureOwnerAsync();
            if (auth != null) return auth;

            if (request == null)
                return GeneralResponse.BadRequest("Invalid request.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
                return GeneralResponse.NotFound("Role not found.");

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            if (!result.Succeeded)
                return GeneralResponse.BadRequest("Failed to remove role.");

            return GeneralResponse.Ok("Role removed.");
        }

        public async Task<GeneralResponse> CheckAccessAsync(CheckAccessRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId))
                return GeneralResponse.BadRequest("Invalid request.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var permissions = await GetPermissionsInternalAsync(user);
            var hasAccess = permissions.Contains(request.Permission);

            return GeneralResponse.Ok("Access checked.", new CheckAccessResponse { HasAccess = hasAccess });
        }

        public async Task<GeneralResponse> ManageRoleAsync(ManageRoleRequest request)
        {
            var auth = await EnsureOwnerAsync();
            if (auth != null) return auth;

            if (request?.RoleData == null || string.IsNullOrWhiteSpace(request.RoleData.Name))
                return GeneralResponse.BadRequest("Invalid role data.");

            var roleName = request.RoleData.Name.Trim();

            if (string.IsNullOrWhiteSpace(request.RoleId))
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                    return GeneralResponse.BadRequest("Role already exists.");

                var newRole = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = roleName };
                var created = await _roleManager.CreateAsync(newRole);

                if (!created.Succeeded)
                    return GeneralResponse.BadRequest("Role creation failed.");

                await SetRolePermissionsAsync(newRole, request.RoleData.Permissions);
                return GeneralResponse.Ok("Role created.", await MapRoleDtoAsync(newRole));
            }

            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
                return GeneralResponse.NotFound("Role not found.");

            role.Name = roleName;
            await _roleManager.UpdateAsync(role);
            await SetRolePermissionsAsync(role, request.RoleData.Permissions);

            return GeneralResponse.Ok("Role updated.", await MapRoleDtoAsync(role));
        }

        public async Task<GeneralResponse> UpdateRolePermissionsAsync(Guid roleId, UpdatePermissionsRequest request)
        {
            var auth = await EnsureOwnerAsync();
            if (auth != null) return auth;

            if (roleId == Guid.Empty || request == null)
                return GeneralResponse.BadRequest("Invalid request.");

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return GeneralResponse.NotFound("Role not found.");

            await SetRolePermissionsAsync(role, request.Permissions);
            return GeneralResponse.Ok("Role permissions updated.", await MapRoleDtoAsync(role));
        }

        public async Task<GeneralResponse> DeleteRoleAsync(Guid roleId)
        {
            var auth = await EnsureOwnerAsync();
            if (auth != null) return auth;

            if (roleId == Guid.Empty)
                return GeneralResponse.BadRequest("Invalid role id.");

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return GeneralResponse.NotFound("Role not found.");

            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            if (users.Any())
                return GeneralResponse.BadRequest("Role is assigned to users.");

            await _roleManager.DeleteAsync(role);
            return GeneralResponse.Ok("Role deleted.");
        }

        // ====== CreateAccount uses explicit DB transaction to ensure atomicity ======
        private async Task<GeneralResponse> CreateAccountAsync(UpsertAccountRequest request, string roleName)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _userManager.FindByEmailAsync(request.Email) != null)
                    return GeneralResponse.BadRequest("Email already exists.");

                var userId = Guid.NewGuid();
                var baseUser = new User
                {
                    Id = userId,
                    UserName = Guid.NewGuid().ToString(),
                    Email = request.Email,
                    name = request.Name,
                    PhoneNumber = request.Phone,
                    gender = request.Gender,
                    created_at = DateTime.UtcNow,
                    JoinedAt = DateTime.UtcNow,
                    role = roleName.Equals("Teacher", StringComparison.OrdinalIgnoreCase) ? TeacherRole :
                           roleName.Equals("Student", StringComparison.OrdinalIgnoreCase) ? StudentRole :
                           roleName.Equals("Parent", StringComparison.OrdinalIgnoreCase) ? ParentRole :
                           SupervisorRole
                };

                User userToCreate = roleName switch
                {
                    var r when r.Equals("Supervisor", StringComparison.OrdinalIgnoreCase) => new Supervisor
                    {
                        Id = baseUser.Id,
                        UserName = baseUser.UserName,
                        Email = baseUser.Email,
                        name = baseUser.name,
                        PhoneNumber = baseUser.PhoneNumber,
                        gender = baseUser.gender,
                        created_at = baseUser.created_at,
                        JoinedAt = baseUser.JoinedAt,
                        role = baseUser.role,
                        MosqueId = request.MosqueId ?? Guid.Empty,
                        assigned_at = request.AssignedAt ?? DateTime.UtcNow
                    },
                    var r when r.Equals("Teacher", StringComparison.OrdinalIgnoreCase) => new Teacher
                    {
                        Id = baseUser.Id,
                        UserName = baseUser.UserName,
                        Email = baseUser.Email,
                        name = baseUser.name,
                        PhoneNumber = baseUser.PhoneNumber,
                        gender = baseUser.gender,
                        created_at = baseUser.created_at,
                        JoinedAt = baseUser.JoinedAt,
                        role = baseUser.role,
                        MosqueId = request.MosqueId ?? Guid.Empty,
                        Bio = string.Empty,
                        assigned_at = request.AssignedAt?.ToString("yyyy-MM-dd") ?? string.Empty
                    },
                    var r when r.Equals("Parent", StringComparison.OrdinalIgnoreCase) => await BuildParentAsync(baseUser, request),
                    _ => new Student
                    {
                        Id = baseUser.Id,
                        UserName = baseUser.UserName,
                        Email = baseUser.Email,
                        name = baseUser.name,
                        PhoneNumber = baseUser.PhoneNumber,
                        gender = baseUser.gender,
                        created_at = baseUser.created_at,
                        JoinedAt = baseUser.JoinedAt,
                        role = baseUser.role,
                        MosqueId = request.MosqueId ?? Guid.Empty,
                        SaturdayHalqeId = Guid.Empty,
                        EnrollmentDate = DateTime.UtcNow,
                        status = request.IsActive ? 0 : 1,
                        score = 0
                    }
                };

                var result = await _userManager.CreateAsync(userToCreate, request.Password ?? "Password123!");
                if (!result.Succeeded)
                    return GeneralResponse.BadRequest("Create user failed.");

                var addRoleRes = await _userManager.AddToRoleAsync(userToCreate, roleName);
                if (!addRoleRes.Succeeded)
                {
                    // rollback through transaction
                    await transaction.RollbackAsync();
                    return GeneralResponse.BadRequest("Assign role failed.");
                }

                // set account status explicitly
                await _userManager.SetLockoutEnabledAsync(userToCreate, true);
                var lockoutEnd = request.IsActive ? (DateTimeOffset?)null : DateTimeOffset.UtcNow.AddYears(100);
                await _userManager.SetLockoutEndDateAsync(userToCreate, lockoutEnd);

                // persist any repository changes
                await _unitOfWork.CompleteAsync();

                await transaction.CommitAsync();
                return GeneralResponse.Ok("Account created.", await MapAccountDetailsAsync(userToCreate, roleName));
            }
            catch
            {
                try { await transaction.RollbackAsync(); } catch { /* ignore */ }
                return GeneralResponse.InternalError("Create account failed.");
            }
        }

        private async Task<GeneralResponse> UpdateAccountAsync(UpsertAccountRequest request, string roleName)
        {
            var user = await _userManager.FindByIdAsync(request.UserId!.Value.ToString());
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            user.name = request.Name;
            user.Email = request.Email;
            user.PhoneNumber = request.Phone;
            user.gender = request.Gender;
            user.theme = request.Relationship;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return GeneralResponse.BadRequest("Update user failed.");

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                // Changing password via UserManager requires token or Remove/Add sequence for local accounts.
                if (await _userManager.HasPasswordAsync(user))
                {
                    var removeRes = await _userManager.RemovePasswordAsync(user);
                    if (!removeRes.Succeeded)
                        return GeneralResponse.BadRequest("Failed to update password.");
                }

                var addRes = await _userManager.AddPasswordAsync(user, request.Password);
                if (!addRes.Succeeded)
                    return GeneralResponse.BadRequest("Failed to update password.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(roleName))
            {
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                var addRoleRes = await _userManager.AddToRoleAsync(user, roleName);
                if (!addRoleRes.Succeeded)
                    return GeneralResponse.BadRequest("Failed to change role.");
            }

            // update status
            await UpdateAccountStatusAsync(new UpdateAccountStatusRequest { UserId = user.Id, IsActive = request.IsActive });

            // update related entity data using repositories
            if (roleName.Equals("Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                var sup = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(user.Id);
                if (sup != null)
                {
                    sup.MosqueId = request.MosqueId ?? sup.MosqueId;
                    sup.assigned_at = request.AssignedAt ?? sup.assigned_at;
                    await _unitOfWork.Repository<Supervisor>().UpdateAsync(sup);
                }
            }
            else if (roleName.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
            {
                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(user.Id);
                if (teacher != null)
                {
                    teacher.MosqueId = request.MosqueId ?? teacher.MosqueId;
                    teacher.assigned_at = request.AssignedAt?.ToString("yyyy-MM-dd") ?? teacher.assigned_at;
                    await _unitOfWork.Repository<Teacher>().UpdateAsync(teacher);
                }
            }
            else if (roleName.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                var student = await _unitOfWork.Repository<Student>().GetByIdAsync(user.Id);
                if (student != null)
                {
                    student.MosqueId = request.MosqueId ?? student.MosqueId;
                    student.status = request.IsActive ? 0 : 1;
                    await _unitOfWork.Repository<Student>().UpdateAsync(student);
                }
            }
            else if (roleName.Equals("Parent", StringComparison.OrdinalIgnoreCase))
            {
                // Parent stored as Student entity variant in this schema
                var parent = await _unitOfWork.Repository<Student>().GetByIdAsync(user.Id);
                if (parent != null)
                {
                    parent.MosqueId = request.MosqueId ?? parent.MosqueId;
                    await _unitOfWork.Repository<Student>().UpdateAsync(parent);
                }
            }

            await _unitOfWork.CompleteAsync();
            return GeneralResponse.Ok("Account updated.", await MapAccountDetailsAsync(user, roleName));
        }

        private async Task<Student> BuildParentAsync(User baseUser, UpsertAccountRequest request)
        {
            if (!request.StudentId.HasValue)
                return new Student
                {
                    Id = baseUser.Id,
                    UserName = baseUser.UserName,
                    Email = baseUser.Email,
                    name = baseUser.name,
                    PhoneNumber = baseUser.PhoneNumber,
                    gender = baseUser.gender,
                    created_at = baseUser.created_at,
                    JoinedAt = baseUser.JoinedAt,
                    role = ParentRole,
                    MosqueId = Guid.Empty,
                    SaturdayHalqeId = Guid.Empty,
                    EnrollmentDate = DateTime.UtcNow,
                    status = request.IsActive ? 0 : 1,
                    score = 0
                };

            var child = await _unitOfWork.Repository<Student>().GetByIdAsync(request.StudentId.Value);
            var parent = new Student
            {
                Id = baseUser.Id,
                UserName = baseUser.UserName,
                Email = baseUser.Email,
                name = baseUser.name,
                PhoneNumber = baseUser.PhoneNumber,
                gender = baseUser.gender,
                created_at = baseUser.created_at,
                JoinedAt = baseUser.JoinedAt,
                role = ParentRole,
                MosqueId = child?.MosqueId ?? Guid.Empty,
                SaturdayHalqeId = child?.SaturdayHalqeId ?? Guid.Empty,
                EnrollmentDate = DateTime.UtcNow,
                status = request.IsActive ? 0 : 1,
                score = 0
            };

            if (child != null)
            {
                child.ParentId = parent.Id;
                await _unitOfWork.Repository<Student>().UpdateAsync(child);
            }

            return parent;
        }

        public async Task<GeneralResponse> GetUserPermissionsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return GeneralResponse.BadRequest("Invalid user id.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var permissions = await GetPermissionsInternalAsync(user);
            var result = permissions.Select(p => new PermissionDto { Key = p, Description = null }).ToList();

            return GeneralResponse.Ok("Permissions retrieved.", result);
        }

        // Helpers

        private async Task<GeneralResponse?> EnsureOwnerAsync()
        {
            var currentId = _currentUserService.CurrentUserId;
            if (!currentId.HasValue)
                return GeneralResponse.Unauthorized("Unauthorized.");

            var user = await _userManager.FindByIdAsync(currentId.Value.ToString());
            if (user == null)
                return GeneralResponse.Unauthorized("Unauthorized.");

            if (!await _userManager.IsInRoleAsync(user, "Owner"))
                return GeneralResponse.Unauthorized("Unauthorized.");

            return null;
        }

        private static bool IsActive(User user)
            => !user.LockoutEnd.HasValue || user.LockoutEnd.Value <= DateTimeOffset.UtcNow;

        private async Task<List<AccountSummaryDto>> MapAccountsAsync(string role, List<User> users)
        {
            var userIds = users.Select(u => u.Id).ToHashSet();

            if (role.Equals("Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                var supervisors = (await _unitOfWork.Repository<Supervisor>().GetAllAsync())
                    .Where(s => userIds.Contains(s.Id))
                    .ToDictionary(s => s.Id);

                var mosqueIds = supervisors.Values.Select(s => s.MosqueId).Distinct().ToList();
                var mosques = (await _unitOfWork.Repository<Mosque>().GetAllAsync())
                    .Where(m => mosqueIds.Contains(m.Id))
                    .ToDictionary(m => m.Id, m => m.name);

                return users.Select(u =>
                {
                    supervisors.TryGetValue(u.Id, out var sup);
                    return new AccountSummaryDto
                    {
                        UserId = u.Id,
                        Name = u.name ?? string.Empty,
                        MosqueName = sup != null && mosques.TryGetValue(sup.MosqueId, out var m) ? m : null,
                        IsActive = IsActive(u)
                    };
                }).ToList();
            }

            if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
            {
                var teachers = (await _unitOfWork.Repository<Teacher>().GetAllAsync())
                    .Where(t => userIds.Contains(t.Id))
                    .ToDictionary(t => t.Id);

                var mosqueIds = teachers.Values.Select(t => t.MosqueId).Distinct().ToList();
                var mosques = (await _unitOfWork.Repository<Mosque>().GetAllAsync())
                    .Where(m => mosqueIds.Contains(m.Id))
                    .ToDictionary(m => m.Id, m => m.name);

                var halqas = (await _unitOfWork.Repository<Halqa>().GetAllAsync())
                    .Where(h => userIds.Contains((Guid)h.TeacherId))
                    .ToList();

                var foujIds = halqas.Select(h => h.FoujId).Distinct().ToList();
                var foujs = (await _unitOfWork.Repository<Fouj>().GetAllAsync())
                    .Where(f => foujIds.Contains(f.Id))
                    .ToDictionary(f => f.Id, f => f.name);

                var foujByTeacher = halqas
                    .GroupBy(h => h.TeacherId)
                    .ToDictionary(g => g.Key, g => foujs.TryGetValue(g.First().FoujId, out var f) ? f : null);

                return users.Select(u =>
                {
                    teachers.TryGetValue(u.Id, out var teacher);
                    foujByTeacher.TryGetValue(u.Id, out var foujName);

                    return new AccountSummaryDto
                    {
                        UserId = u.Id,
                        Name = u.name ?? string.Empty,
                        MosqueName = teacher != null && mosques.TryGetValue(teacher.MosqueId, out var m) ? m : null,
                        FoujName = foujName,
                        IsActive = IsActive(u)
                    };
                }).ToList();
            }

            if (role.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                var students = (await _unitOfWork.Repository<Student>().GetAllAsync())
                    .Where(s => userIds.Contains(s.Id))
                    .ToDictionary(s => s.Id);

                var mosqueIds = students.Values.Select(s => s.MosqueId).Distinct().ToList();
                var mosques = (await _unitOfWork.Repository<Mosque>().GetAllAsync())
                    .Where(m => mosqueIds.Contains(m.Id))
                    .ToDictionary(m => m.Id, m => m.name);

                return users.Select(u =>
                {
                    students.TryGetValue(u.Id, out var student);
                    return new AccountSummaryDto
                    {
                        UserId = u.Id,
                        Name = u.name ?? string.Empty,
                        MosqueName = student != null && mosques.TryGetValue(student.MosqueId, out var m) ? m : null,
                        IsActive = IsActive(u)
                    };
                }).ToList();
            }

            if (role.Equals("Parent", StringComparison.OrdinalIgnoreCase))
            {
                var parents = (await _unitOfWork.Repository<Student>().GetAllAsync())
                    .Where(s => userIds.Contains(s.Id))
                    .ToDictionary(s => s.Id);

                var parentIds = parents.Keys.ToHashSet();
                var children = (await _unitOfWork.Repository<Student>().GetAllAsync())
                    .Where(s => s.ParentId.HasValue && parentIds.Contains(s.ParentId.Value))
                    .ToList();

                var mosqueIds = parents.Values.Select(p => p.MosqueId).Distinct().ToList();
                var mosques = (await _unitOfWork.Repository<Mosque>().GetAllAsync())
                    .Where(m => mosqueIds.Contains(m.Id))
                    .ToDictionary(m => m.Id, m => m.name);

                var childrenLookup = children
                    .GroupBy(c => c.ParentId!.Value)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.name ?? string.Empty).ToList());

                return users.Select(u =>
                {
                    parents.TryGetValue(u.Id, out var parent);
                    childrenLookup.TryGetValue(u.Id, out var list);

                    return new AccountSummaryDto
                    {
                        UserId = u.Id,
                        Name = u.name ?? string.Empty,
                        MosqueName = parent != null && mosques.TryGetValue(parent.MosqueId, out var m) ? m : null,
                        ChildrenNames = list ?? new List<string>(),
                        IsActive = IsActive(u)
                    };
                }).ToList();
            }

            // default mapping
            return users.Select(u => new AccountSummaryDto
            {
                UserId = u.Id,
                Name = u.name ?? string.Empty,
                IsActive = IsActive(u)
            }).ToList();
        }

        private async Task<AccountDetailsDto> MapAccountDetailsAsync(User user, string role)
        {
            var dto = new AccountDetailsDto
            {
                UserId = user.Id,
                Role = role,
                Name = user.name ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber,
                Gender = user.gender ?? string.Empty,
                IsActive = IsActive(user),
                Relationship = user.theme
            };

            if (role.Equals("Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                var sup = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(user.Id);
                if (sup != null)
                {
                    dto.MosqueId = sup.MosqueId;
                    dto.AssignedAt = sup.assigned_at;
                }
            }
            else if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
            {
                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(user.Id);
                if (teacher != null)
                {
                    dto.MosqueId = teacher.MosqueId;
                    if (DateTime.TryParse(teacher.assigned_at, out var dt))
                        dto.AssignedAt = dt;
                }
            }
            else if (role.Equals("Student", StringComparison.OrdinalIgnoreCase) || role.Equals("Parent", StringComparison.OrdinalIgnoreCase))
            {
                var student = await _unitOfWork.Repository<Student>().GetByIdAsync(user.Id);
                if (student != null)
                {
                    dto.MosqueId = student.MosqueId;
                    dto.StudentId = student.ParentId;
                }
            }

            return dto;
        }

        private async Task<List<string>> GetPermissionsInternalAsync(User user)
        {
            var permissions = new HashSet<string>();
            var userClaims = await _userManager.GetClaimsAsync(user);

            foreach (var c in userClaims.Where(c => c.Type == PermissionClaimType))
                permissions.Add(c.Value);

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) continue;

                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var c in claims.Where(c => c.Type == PermissionClaimType))
                    permissions.Add(c.Value);
            }

            return permissions.ToList();
        }

        private async Task<RoleDto> MapRoleDtoAsync(IdentityRole<Guid> role)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims.Where(c => c.Type == PermissionClaimType)
                                    .Select(c => c.Value)
                                    .Distinct()
                                    .ToList();

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Permissions = permissions
            };
        }

        private async Task SetRolePermissionsAsync(IdentityRole<Guid> role, List<string>? permissions)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var c in claims.Where(c => c.Type == PermissionClaimType))
                await _roleManager.RemoveClaimAsync(role, c);

            if (permissions == null) return;

            foreach (var permission in permissions.Distinct())
                await _roleManager.AddClaimAsync(role, new Claim(PermissionClaimType, permission));
        }

        public async Task<GeneralResponse> GetUserRolesAsync(UserRolesRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId))
                return GeneralResponse.BadRequest("Invalid request.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var roleNames = await _userManager.GetRolesAsync(user);
            var result = new List<RoleDto>();

            // جلب تفاصيل كل دور مع صلاحياته
            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                    result.Add(await MapRoleDtoAsync(role));
            }

            return GeneralResponse.Ok("User roles retrieved.", new UserRolesResponse { Roles = result });
        }

        public async Task<GeneralResponse> GetAllRolesAsync(RoleFilter filter)
        {
            filter ??= new RoleFilter();

            // نبدأ من الأدوار كـ IQueryable للفلترة الأساسية
            var query = _roleManager.Roles.AsQueryable();

            // فلترة باسم الدور
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var name = filter.Name.Trim();
                query = query.Where(r => (r.Name ?? string.Empty).Contains(name));
            }

            // فلترة بالصلاحيات (تتطلب جلب الـ Claims، لذا ننفذها بعد التحميل)
            if (!string.IsNullOrWhiteSpace(filter.Permission))
            {
                var permission = filter.Permission.Trim();
                var allRoles = await query.ToListAsync();
                var filtered = new List<IdentityRole<Guid>>();

                foreach (var role in allRoles)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    if (claims.Any(c => c.Type == PermissionClaimType && c.Value == permission))
                        filtered.Add(role);
                }
                // نعيد التصفح على القائمة المفلترة
                var total = filtered.Count;
                var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
                var pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;

                var paged = filtered
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var dtos = new List<RoleDto>();
                foreach (var role in paged)
                    dtos.Add(await MapRoleDtoAsync(role));

                return GeneralResponse.Ok("Roles retrieved.", dtos, pageNumber, pageSize, total);
            }

            // إذا لم تكن هناك فلترة معقدة، نكمل مع IQueryable
            var totalRecords = await query.CountAsync();
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var size = filter.PageSize <= 0 ? 20 : filter.PageSize;

            var roles = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var resultDtos = new List<RoleDto>();
            foreach (var role in roles)
                resultDtos.Add(await MapRoleDtoAsync(role));

            return GeneralResponse.Ok("Roles retrieved.", resultDtos, page, size, totalRecords);
        }

        public async Task<GeneralResponse> GetRoleByIdAsync(Guid roleId)
        {
            if (roleId == Guid.Empty)
                return GeneralResponse.BadRequest("Invalid role id.");

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return GeneralResponse.NotFound("Role not found.");

            return GeneralResponse.Ok("Role retrieved.", await MapRoleDtoAsync(role));
        }

        public async Task<GeneralResponse> GetAllPermissionsAsync()
        {
            // نجمع كل الصلاحيات الفريدة من جميع الأدوار
            var permissions = new HashSet<string>();
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in claims.Where(c => c.Type == PermissionClaimType))
                {
                    permissions.Add(claim.Value);
                }
            }

            var result = permissions
                .Select(p => new PermissionDto { Key = p, Description = null })
                .OrderBy(p => p.Key)
                .ToList();

            return GeneralResponse.Ok("Permissions retrieved.", result);
        }
    }
}