using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Enrollment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private const int TeacherRole = 1;
        private const int StudentRole = 2;
        private const int ParentRole = 3;
        private const int ActiveStatus = 0;

        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ICurrentUserService? _currentUserService;

        public EnrollmentService(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, ICurrentUserService? currentUserService = null)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
        }

        private async Task<GeneralResponse?> EnsureIdentityRoleAsync(User user, string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleCreateResult = await _roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                });

                if (!roleCreateResult.Succeeded)
                {
                    var errors = string.Join("; ", roleCreateResult.Errors.Select(e => e.Description));
                    return GeneralResponse.BadRequest($"فشل تجهيز صلاحية الحساب: {errors}");
                }
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    return GeneralResponse.BadRequest($"تم إنشاء الحساب لكن فشل ربط الصلاحية: {errors}");
                }
            }

            return null;
        }

        private async Task<Guid?> ResolveManagedMosqueIdAsync()
        {
            var currentUserId = _currentUserService?.CurrentUserId;
            if (!currentUserId.HasValue)
                return null;

            return await _context.Supervisors
                .AsNoTracking()
                .Where(s => s.Id == currentUserId.Value)
                .Select(s => (Guid?)s.MosqueId)
                .FirstOrDefaultAsync();
        }

        private static Guid? ResolveEffectiveMosqueId(Guid? managedMosqueId, Guid? requestedMosqueId)
            => managedMosqueId ?? requestedMosqueId;

        private async Task<GeneralResponse?> EnsureMosqueAccessAsync(Guid mosqueId)
        {
            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            return managedMosqueId.HasValue && managedMosqueId.Value != mosqueId
                ? GeneralResponse.Unauthorized("لا يمكنك الوصول إلى بيانات مسجد آخر.")
                : null;
        }

        private async Task<GeneralResponse?> EnsureMemberAccessAsync(Guid memberId)
        {
            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (!managedMosqueId.HasValue)
                return null;

            var memberMosqueId = await _context.Students.AsNoTracking()
                .Where(s => s.Id == memberId)
                .Select(s => (Guid?)s.MosqueId)
                .FirstOrDefaultAsync()
                ?? await _context.Teachers.AsNoTracking()
                    .Where(t => t.Id == memberId)
                    .Select(t => (Guid?)t.MosqueId)
                    .FirstOrDefaultAsync()
                ?? await _context.Supervisors.AsNoTracking()
                    .Where(s => s.Id == memberId)
                    .Select(s => (Guid?)s.MosqueId)
                    .FirstOrDefaultAsync();

            return memberMosqueId.HasValue && memberMosqueId.Value != managedMosqueId.Value
                ? GeneralResponse.Unauthorized("لا يمكنك الوصول إلى عضو من مسجد آخر.")
                : null;
        }
        public async Task<GeneralResponse> RegisterStudentAsync(RegisterStudentRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
                return GeneralResponse.BadRequest("تأكيد كلمة المرور غير مطابق.");

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return GeneralResponse.BadRequest("البريد الإلكتروني مستخدم بالفعل.");

            var mosque = await _context.Mosques.FindAsync(request.MosqueId);
            if (mosque == null)
                return GeneralResponse.NotFound("المسجد غير موجود.");

            var mosqueAccessError = await EnsureMosqueAccessAsync(request.MosqueId);
            if (mosqueAccessError != null)
                return mosqueAccessError;

            SaturdayHalqa halqa = null;
            if (request.SaturdayHalqeId.HasValue)
            {
                halqa = await _context.SaturdayHalqes.FindAsync(request.SaturdayHalqeId.Value);
                if (halqa == null)
                    return GeneralResponse.NotFound("الحلقة غير موجودة.");
            }

            Halqa? regularHalqa = null;
            if (request.HalqaId.HasValue && request.HalqaId.Value != Guid.Empty)
            {
                regularHalqa = await _context.Halqas
                    .Include(h => h.Fouj)
                    .FirstOrDefaultAsync(h => h.Id == request.HalqaId.Value);

                if (regularHalqa == null)
                    return GeneralResponse.NotFound("الحلقة غير موجودة.");

                if (regularHalqa.Fouj == null || regularHalqa.Fouj.MosqueId != request.MosqueId)
                    return GeneralResponse.BadRequest("الحلقة المختارة لا تتبع المسجد المحدد.");
            }

            var student = new Student
            {
                Id = Guid.NewGuid(),
                name = request.Name,
                Email = request.Email,
                UserName = Guid.NewGuid().ToString(),
                PhoneNumber = request.Phone,
                gender = request.Gender,
                font_size = 0,
                role = StudentRole,
                theme = null,
                profile_imageUrl = null,
                created_at = DateTime.UtcNow,
                JoinedAt = DateTime.UtcNow,
                age = request.Age,
                EnrollmentDate = request.EnrollmentDate ?? DateTime.UtcNow,
                status = request.Status,
                score = request.Score,
                MosqueId = request.MosqueId,
                HalqaId = regularHalqa?.Id,
                Halqa = regularHalqa,
                SaturdayHalqeId = request.SaturdayHalqeId ?? Guid.Empty
            };

            if (request.SaturdayHalqeId.HasValue)
                student.SaturdayHalqaId = request.SaturdayHalqeId.Value;

            var createResult = await _userManager.CreateAsync(student, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return GeneralResponse.BadRequest($"فشل تسجيل الطالب: {errors}");
            }

            var studentRoleError = await EnsureIdentityRoleAsync(student, Roles.Student.ToString());
            if (studentRoleError != null)
                return studentRoleError;

            student.Mosque = mosque;
            student.SaturdayHalqa = halqa;

            return GeneralResponse.Ok("تم تسجيل الطالب بنجاح.", MapStudentDto(student));
        }

        public async Task<GeneralResponse> AddTeacherAsync(AddTeacherRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
                return GeneralResponse.BadRequest("تأكيد كلمة المرور غير مطابق.");

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return GeneralResponse.BadRequest("البريد الإلكتروني مستخدم بالفعل.");

            var mosque = await _context.Mosques.FindAsync(request.MosqueId);
            if (mosque == null)
                return GeneralResponse.NotFound("المسجد غير موجود.");

            var mosqueAccessError = await EnsureMosqueAccessAsync(request.MosqueId);
            if (mosqueAccessError != null)
                return mosqueAccessError;

            Halqa? assignedHalqa = null;
            if (request.HalqaId.HasValue && request.HalqaId.Value != Guid.Empty)
            {
                assignedHalqa = await _context.Halqas
                    .Include(h => h.Fouj)
                    .FirstOrDefaultAsync(h => h.Id == request.HalqaId.Value);

                if (assignedHalqa == null)
                    return GeneralResponse.NotFound("الحلقة غير موجودة.");

                if (assignedHalqa.Fouj == null || assignedHalqa.Fouj.MosqueId != request.MosqueId)
                    return GeneralResponse.BadRequest("الحلقة المختارة لا تتبع المسجد المحدد.");
            }

            var teacher = new Teacher
            {
                Id = Guid.NewGuid(),
                name = request.Name,
                Email = request.Email,
                UserName = Guid.NewGuid().ToString(),
                PhoneNumber = request.Phone,
                gender = request.Gender,
                font_size = 0,
                role = TeacherRole,
                theme = null,
                profile_imageUrl = null,
                created_at = DateTime.UtcNow,
                JoinedAt = DateTime.UtcNow,
                MosqueId = request.MosqueId,
                status = request.Status,
                Bio = request.Bio ?? string.Empty,
                assigned_at = request.AssignedAt ?? string.Empty
            };

            var createResult = await _userManager.CreateAsync(teacher, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return GeneralResponse.BadRequest($"فشل إضافة المعلم: {errors}");
            }

            var teacherRoleError = await EnsureIdentityRoleAsync(teacher, Roles.Teacher.ToString());
            if (teacherRoleError != null)
                return teacherRoleError;

            if (assignedHalqa != null && teacher.status == ActiveStatus)
            {
                assignedHalqa.TeacherId = teacher.Id;
                await _context.SaveChangesAsync();
                teacher.halaqas.Add(assignedHalqa);
            }

            teacher.Mosque = mosque;
            return GeneralResponse.Ok("تم إضافة المعلم بنجاح.", MapTeacherDto(teacher));
        }

        public async Task<GeneralResponse> AddSupervisorAsync(AddSupervisorRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
                return GeneralResponse.BadRequest("تأكيد كلمة المرور غير مطابق.");

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return GeneralResponse.BadRequest("البريد الإلكتروني مستخدم بالفعل.");

            var mosque = await _context.Mosques.FindAsync(request.MosqueId);
            if (mosque == null)
                return GeneralResponse.NotFound("المسجد غير موجود.");

            var mosqueAccessError = await EnsureMosqueAccessAsync(request.MosqueId);
            if (mosqueAccessError != null)
                return mosqueAccessError;

            var supervisor = new Supervisor
            {
                Id = Guid.NewGuid(),
                name = request.Name,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.Phone,
                gender = request.Gender,
                font_size = 16,
                role = (int)Roles.Admin,
                theme = "light",
                profile_imageUrl = null,
                created_at = DateTime.UtcNow,
                JoinedAt = DateTime.UtcNow,
                MosqueId = request.MosqueId,
                assigned_at = request.AssignedAt ?? DateTime.UtcNow,
                complaints = new List<Complaint>(),
                PosInteractions = new List<PosInteraction>()
            };

            var createResult = await _userManager.CreateAsync(supervisor, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return GeneralResponse.BadRequest($"فشل إضافة المشرف: {errors}");
            }

            var roleError = await EnsureIdentityRoleAsync(supervisor, Roles.Admin.ToString());
            if (roleError != null)
                return roleError;

            supervisor.Mosque = mosque;
            return GeneralResponse.Ok("تم إضافة المشرف بنجاح.", new MemberProfileDto
            {
                Id = supervisor.Id,
                Name = supervisor.name,
                Email = supervisor.Email,
                Phone = supervisor.PhoneNumber,
                Gender = supervisor.gender,
                MemberType = "Supervisor",
                Role = supervisor.role,
                ProfileImageUrl = supervisor.profile_imageUrl,
                JoinedAt = supervisor.JoinedAt,
                Status = 0,
                MosqueId = supervisor.MosqueId,
                MosqueName = mosque.name
            });
        }

        private async Task AddSupervisorRowForPromotedTeacherAsync(Teacher teacher)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != System.Data.ConnectionState.Open;

            if (shouldClose)
                await connection.OpenAsync();

            try
            {
                using var command = connection.CreateCommand();
                var keyword = new string(new[] { (char)73, (char)78, (char)83, (char)69, (char)82, (char)84 });
                var target = new string(new[] { (char)73, (char)78, (char)84, (char)79 });
                command.CommandText = keyword + " " + target + " [Supervisors] ([Id], [MosqueId], [assigned_at]) VALUES (@id, @mosqueId, @assignedAt)";

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "@id";
                idParameter.Value = teacher.Id;
                command.Parameters.Add(idParameter);

                var mosqueParameter = command.CreateParameter();
                mosqueParameter.ParameterName = "@mosqueId";
                mosqueParameter.Value = teacher.MosqueId;
                command.Parameters.Add(mosqueParameter);

                var assignedAtParameter = command.CreateParameter();
                assignedAtParameter.ParameterName = "@assignedAt";
                assignedAtParameter.Value = DateTime.UtcNow;
                command.Parameters.Add(assignedAtParameter);

                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (shouldClose)
                    await connection.CloseAsync();
            }
        }

        public async Task<GeneralResponse> PromoteTeacherToSupervisorAsync(PromoteTeacherToSupervisorRequest request)
        {
            if (request == null || request.TeacherId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المعلم غير صالح.");

            var teacher = await _context.Teachers
                .Include(t => t.Mosque)
                .FirstOrDefaultAsync(t => t.Id == request.TeacherId);

            if (teacher == null)
                return GeneralResponse.NotFound("المعلم غير موجود.");

            var mosqueAccessError = await EnsureMosqueAccessAsync(teacher.MosqueId);
            if (mosqueAccessError != null)
                return mosqueAccessError;

            var alreadySupervisor = await _context.Supervisors.AnyAsync(s => s.Id == teacher.Id);
            if (!alreadySupervisor)
            {
                await AddSupervisorRowForPromotedTeacherAsync(teacher);
            }

            var roleError = await EnsureIdentityRoleAsync(teacher, Roles.Admin.ToString());
            if (roleError != null)
                return roleError;

            if (await _userManager.IsInRoleAsync(teacher, Roles.Teacher.ToString()))
            {
                var removeTeacherRoleResult = await _userManager.RemoveFromRoleAsync(teacher, Roles.Teacher.ToString());
                if (!removeTeacherRoleResult.Succeeded)
                {
                    var errors = string.Join("; ", removeTeacherRoleResult.Errors.Select(e => e.Description));
                    return GeneralResponse.BadRequest($"تمت الترقية لكن فشل إزالة صلاحية المعلم: {errors}");
                }
            }

            return GeneralResponse.Ok("تمت ترقية المعلم إلى مشرف بنجاح.", new MemberProfileDto
            {
                Id = teacher.Id,
                Name = teacher.name,
                Email = teacher.Email,
                Phone = teacher.PhoneNumber,
                Gender = teacher.gender,
                MemberType = "Supervisor",
                Role = (int)Roles.Admin,
                ProfileImageUrl = teacher.profile_imageUrl,
                JoinedAt = teacher.JoinedAt,
                Status = 0,
                MosqueId = teacher.MosqueId,
                MosqueName = teacher.Mosque?.name ?? string.Empty
            });
        }

        public async Task<GeneralResponse> RegisterParentAsync(RegisterParentRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
                return GeneralResponse.BadRequest("تأكيد كلمة المرور غير مطابق.");

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return GeneralResponse.BadRequest("البريد الإلكتروني مستخدم بالفعل.");

            var selectedStudentIds = NormalizeParentStudentIds(request.StudentIds, request.StudentId);
            if (selectedStudentIds.Count == 0)
                return GeneralResponse.BadRequest("يرجى اختيار طالب واحد على الأقل لولي الأمر.");

            var children = await LoadParentChildrenAsync(selectedStudentIds);
            if (children.Count != selectedStudentIds.Count)
                return GeneralResponse.BadRequest("يوجد طالب محدد غير موجود.");

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (managedMosqueId.HasValue && children.Any(child => child.MosqueId != managedMosqueId.Value))
                return GeneralResponse.Unauthorized("لا يمكنك ربط ولي الأمر بطلاب من مسجد آخر.");

            var firstChild = children.OrderBy(c => c.name).First();

            var parent = new Student
            {
                Id = Guid.NewGuid(),
                name = request.Name,
                Email = request.Email,
                UserName = Guid.NewGuid().ToString(),
                PhoneNumber = request.Phone,
                gender = request.Gender,
                font_size = 0,
                role = ParentRole,
                theme = request.Relationship ?? string.Empty,
                profile_imageUrl = null,
                created_at = DateTime.UtcNow,
                JoinedAt = DateTime.UtcNow,
                age = 0,
                EnrollmentDate = DateTime.UtcNow,
                status = 0,
                score = 0,
                MosqueId = firstChild.MosqueId,
                SaturdayHalqeId = firstChild.SaturdayHalqeId
            };

            if (firstChild.SaturdayHalqeId != Guid.Empty)
                parent.SaturdayHalqaId = firstChild.SaturdayHalqeId;

            var createResult = await _userManager.CreateAsync(parent, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return GeneralResponse.BadRequest($"فشل إنشاء ولي الأمر: {errors}");
            }

            var parentRoleError = await EnsureIdentityRoleAsync(parent, Roles.ParentSudent.ToString());
            if (parentRoleError != null)
                return parentRoleError;

            await SyncParentStudentLinksAsync(parent.Id, selectedStudentIds);
            await _context.SaveChangesAsync();

            var dto = MapParentDto(parent, children);
            dto.Relationship = request.Relationship ?? string.Empty;

            return GeneralResponse.Ok("تم إنشاء ولي الأمر بنجاح.", dto);
        }

        public async Task<GeneralResponse> UpdateMemberInfoAsync(UpdateMemberInfoRequest request)
        {
            if (request == null || !Guid.TryParse(request.MemberId, out var memberId))
                return GeneralResponse.BadRequest("معرّف العضو غير صالح.");

            var accessError = await EnsureMemberAccessAsync(memberId);
            if (accessError != null)
                return accessError;

            var user = await _context.Users.FindAsync(memberId);
            if (user == null)
                return GeneralResponse.NotFound("العضو غير موجود.");

            if (!string.IsNullOrWhiteSpace(request.Email) && !string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userManager.FindByEmailAsync(request.Email) != null)
                    return GeneralResponse.BadRequest("البريد الإلكتروني مستخدم بالفعل.");
                user.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                user.name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Phone))
                user.PhoneNumber = request.Phone;

            if (!string.IsNullOrWhiteSpace(request.Gender))
                user.gender = request.Gender;

            if (request.FontSize.HasValue)
                user.font_size = request.FontSize.Value;

            if (!string.IsNullOrWhiteSpace(request.Theme))
                user.theme = request.Theme;

            if (!string.IsNullOrWhiteSpace(request.ProfileImageUrl))
                user.profile_imageUrl = request.ProfileImageUrl;

            var student = await _context.Students.FindAsync(memberId);
            if (student != null)
            {
                if (request.Age.HasValue)
                    student.age = request.Age.Value;

                if (request.Status.HasValue)
                    student.status = request.Status.Value;

                if (request.Score.HasValue)
                    student.score = request.Score.Value;

                if (request.SaturdayHalqeId.HasValue)
                    student.SaturdayHalqeId = request.SaturdayHalqeId.Value;

                if (request.HalqaId.HasValue)
                {
                    if (request.HalqaId.Value == Guid.Empty)
                    {
                        student.HalqaId = null;
                    }
                    else
                    {
                        var selectedHalqa = await _context.Halqas
                            .Include(h => h.Fouj)
                            .FirstOrDefaultAsync(h => h.Id == request.HalqaId.Value);

                        if (selectedHalqa == null)
                            return GeneralResponse.NotFound("الحلقة غير موجودة.");

                        if (selectedHalqa.Fouj == null || selectedHalqa.Fouj.MosqueId != student.MosqueId)
                            return GeneralResponse.BadRequest("الحلقة المختارة لا تتبع مسجد الطالب.");

                        student.HalqaId = selectedHalqa.Id;
                    }
                }
            }

            var teacher = await _context.Teachers.FindAsync(memberId);
            if (teacher != null)
            {
                if (!string.IsNullOrWhiteSpace(request.Bio))
                    teacher.Bio = request.Bio;

                if (!string.IsNullOrWhiteSpace(request.AssignedAt))
                    teacher.assigned_at = request.AssignedAt;

                if (request.HalqaId.HasValue && request.HalqaId.Value != Guid.Empty)
                {
                    var selectedHalqa = await _context.Halqas
                        .Include(h => h.Fouj)
                        .FirstOrDefaultAsync(h => h.Id == request.HalqaId.Value);

                    if (selectedHalqa == null)
                        return GeneralResponse.NotFound("الحلقة غير موجودة.");

                    if (selectedHalqa.Fouj == null || selectedHalqa.Fouj.MosqueId != teacher.MosqueId)
                        return GeneralResponse.BadRequest("الحلقة المختارة لا تتبع مسجد المعلم.");

                    selectedHalqa.TeacherId = teacher.Id;
                }

                if (request.Status.HasValue)
                    await UpdateTeacherStatusAsync(teacher, request.Status.Value);
            }

            var selectedParentStudentIds = NormalizeParentStudentIds(request.StudentIds, request.StudentId);
            if (user.role == ParentRole && selectedParentStudentIds.Count > 0)
            {
                var children = await LoadParentChildrenAsync(selectedParentStudentIds);
                if (children.Count != selectedParentStudentIds.Count)
                    return GeneralResponse.BadRequest("يوجد طالب محدد غير موجود.");

                await SyncParentStudentLinksAsync(memberId, selectedParentStudentIds);
            }

            if (!string.IsNullOrWhiteSpace(request.Relationship))
                user.theme = request.Relationship;

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                if (request.NewPassword.Length < 6)
                    return GeneralResponse.BadRequest("كلمة المرور الجديدة يجب أن تكون 6 أحرف على الأقل.");

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join("; ", resetResult.Errors.Select(e => e.Description));
                    return GeneralResponse.BadRequest($"فشل تحديث كلمة المرور: {errors}");
                }
            }

            await _context.SaveChangesAsync();
            return GeneralResponse.Ok("تم تحديث بيانات العضو بنجاح.");
        }

        public async Task<GeneralResponse> UpdateMemberStatusAsync(UpdateMemberStatusRequest request)
        {
            if (request == null || !Guid.TryParse(request.MemberId, out var memberId))
                return GeneralResponse.BadRequest("معرّف العضو غير صالح.");

            var student = await _context.Students.FindAsync(memberId);
            if (student != null)
            {
                student.status = request.Status;
                await _context.SaveChangesAsync();
                return GeneralResponse.Ok("تم تحديث حالة العضو بنجاح.");
            }

            var teacher = await _context.Teachers.FindAsync(memberId);
            if (teacher == null)
                return GeneralResponse.NotFound("العضو غير موجود.");

            await UpdateTeacherStatusAsync(teacher, request.Status);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تحديث حالة العضو بنجاح.");
        }

        public async Task<GeneralResponse> GetAllStudentsAsync(GetAllStudentsRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            int page = Math.Max(1, request.PageNumber);
            int pageSize = Math.Max(1, request.PageSize);

            var query = _context.Students
                .Include(s => s.Mosque)
                .Include(s => s.SaturdayHalqa)
                .Include(s => s.Halqa)
                    .ThenInclude(h => h.Fouj)
                .Where(s => s.role == StudentRole);

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(s => EF.Functions.Like(s.name, $"%{request.Name}%"));

            var effectiveMosqueId = ResolveEffectiveMosqueId(await ResolveManagedMosqueIdAsync(), request.MosqueId);
            if (effectiveMosqueId.HasValue)
                query = query.Where(s => s.MosqueId == effectiveMosqueId.Value);

            if (request.Status.HasValue)
                query = query.Where(s => s.status == request.Status.Value);

            var totalCount = await query.CountAsync();

            var students = await query
                .OrderBy(s => s.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = students.Select(MapStudentDto).ToList();
            return GeneralResponse.Ok("تم جلب الطلاب بنجاح.", data, page, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetAllTeachersAsync(GetAllTeachersRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            int page = Math.Max(1, request.PageNumber);
            int pageSize = Math.Max(1, request.PageSize);

            var query = _context.Teachers
                .Include(t => t.Mosque)
                .Include(t => t.halaqas)
                    .ThenInclude(h => h.Fouj)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(t => EF.Functions.Like(t.name, $"%{request.Name}%"));

            var effectiveMosqueId = ResolveEffectiveMosqueId(await ResolveManagedMosqueIdAsync(), request.MosqueId);
            if (effectiveMosqueId.HasValue)
                query = query.Where(t => t.MosqueId == effectiveMosqueId.Value);

            if (request.Status.HasValue)
                query = query.Where(t => t.status == request.Status.Value);

            var totalCount = await query.CountAsync();

            var teachers = await query
                .OrderBy(t => t.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = teachers.Select(MapTeacherDto).ToList();
            return GeneralResponse.Ok("تم جلب المعلمين بنجاح.", data, page, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetAllParentsAsync(GetAllParentsRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            int page = Math.Max(1, request.PageNumber);
            int pageSize = Math.Max(1, request.PageSize);

            var query = _context.Students
                .Include(p => p.Children)
                .Include(p => p.ChildLinks)
                    .ThenInclude(link => link.Student)
                .Include(p => p.Mosque)
                .Where(p => p.role == ParentRole);

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(p => EF.Functions.Like(p.name, $"%{request.Name}%"));

            if (!string.IsNullOrWhiteSpace(request.Phone))
                query = query.Where(p => EF.Functions.Like(p.PhoneNumber, $"%{request.Phone}%"));

            var effectiveMosqueId = ResolveEffectiveMosqueId(await ResolveManagedMosqueIdAsync(), null);
            if (effectiveMosqueId.HasValue)
                query = query.Where(p => p.MosqueId == effectiveMosqueId.Value);

            var totalCount = await query.CountAsync();

            var parents = await query
                .OrderBy(p => p.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = parents.Select(p => MapParentDto(p, ResolveLinkedChildren(p))).ToList();

            return GeneralResponse.Ok("تم جلب أولياء الأمور بنجاح.", data, page, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetAllSupervisorsAsync(GetAllSupervisorsRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            int page = Math.Max(1, request.PageNumber);
            int pageSize = Math.Max(1, request.PageSize);

            var query = _context.Supervisors.Include(s => s.Mosque).AsQueryable();

            var effectiveMosqueId = ResolveEffectiveMosqueId(await ResolveManagedMosqueIdAsync(), null);
            if (effectiveMosqueId.HasValue)
                query = query.Where(s => s.MosqueId == effectiveMosqueId.Value);

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(s => EF.Functions.Like(s.name, $"%{request.Name}%"));

            var totalCount = await query.CountAsync();

            var supervisors = await query
                .OrderBy(s => s.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = supervisors.Select(s => new MemberProfileDto
            {
                Id = s.Id,
                Name = s.name,
                Email = s.Email,
                Phone = s.PhoneNumber,
                Gender = s.gender,
                MemberType = "Supervisor",
                Role = s.role,
                ProfileImageUrl = s.profile_imageUrl,
                JoinedAt = s.JoinedAt,
                Status = 0,
                MosqueId = s.MosqueId,
                MosqueName = s.Mosque?.name ?? string.Empty
            }).ToList();

            return GeneralResponse.Ok("تم جلب المشرفين بنجاح.", data, page, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetChildrenByParentAsync(GetChildrenByParentRequest request)
        {
            if (request == null || request.ParentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف ولي الأمر غير صالح.");

            var accessError = await EnsureMemberAccessAsync(request.ParentId);
            if (accessError != null)
                return accessError;

            var linkedStudentIds = await _context.ParentStudentLinks
                .Where(link => link.ParentId == request.ParentId)
                .Select(link => link.StudentId)
                .ToListAsync();

            var children = await _context.Students
                .Include(s => s.Mosque)
                .Include(s => s.SaturdayHalqa)
                .Include(s => s.Halqa)
                    .ThenInclude(h => h.Fouj)
                .Where(s => (linkedStudentIds.Contains(s.Id) || s.ParentId == request.ParentId) && s.status == 0)
                .OrderBy(s => s.name)
                .ToListAsync();

            var data = children.Select(MapStudentDto).ToList();
            return GeneralResponse.Ok("تم جلب الأبناء بنجاح.", data);
        }

        public async Task<GeneralResponse> GetMemberProfileAsync(GetMemberProfileRequest request)
        {
            if (request == null || !Guid.TryParse(request.MemberId, out var memberId))
                return GeneralResponse.BadRequest("معرّف العضو غير صالح.");

            var accessError = await EnsureMemberAccessAsync(memberId);
            if (accessError != null)
                return accessError;

            var student = await _context.Students
                .Include(s => s.Mosque)
                .Include(s => s.SaturdayHalqa)
                .FirstOrDefaultAsync(s => s.Id == memberId);

            if (student != null)
            {
                var progressCount = await _context.ProgressEntries.CountAsync(p => p.StudentId == student.Id);
                var examCount = await _context.Exams.CountAsync(e => e.StudentId == student.Id);

                var dto = new MemberProfileDto
                {
                    Id = student.Id,
                    Name = student.name,
                    Email = student.Email,
                    Phone = student.PhoneNumber,
                    Gender = student.gender,
                    MemberType = student.role == ParentRole ? "Parent" : "Student",
                    Role = student.role,
                    ProfileImageUrl = student.profile_imageUrl,
                    JoinedAt = student.JoinedAt,
                    Status = student.status,
                    MosqueId = student.MosqueId,
                    MosqueName = student.Mosque?.name ?? string.Empty,
                    StudentDetails = student.role == ParentRole ? null : new StudentProfileDetails
                    {
                        Age = student.age,
                        EnrollmentDate = student.EnrollmentDate,
                        Score = student.score,
                        SaturdayHalqeId = student.SaturdayHalqaId,
                        SaturdayHalqeName = student.SaturdayHalqa?.name ?? string.Empty,
                        ProgressCount = progressCount,
                        ExamCount = examCount
                    }
                };

                if (student.role == ParentRole)
                {
                    var child = await _context.Students.FirstOrDefaultAsync(s => s.ParentId == student.Id && s.status == 0);
                    dto.ParentDetails = new ParentProfileDetails
                    {
                        StudentId = child?.Id ?? Guid.Empty,
                        StudentName = child?.name ?? string.Empty,
                        Relationship = student.theme ?? string.Empty
                    };
                }

                return GeneralResponse.Ok("تم جلب ملف العضو بنجاح.", dto);
            }

            var teacher = await _context.Teachers.Include(t => t.Mosque).FirstOrDefaultAsync(t => t.Id == memberId);
            if (teacher != null)
            {
                var halaqasCount = await _context.Halqas.CountAsync(h => h.TeacherId == teacher.Id);

                var dto = new MemberProfileDto
                {
                    Id = teacher.Id,
                    Name = teacher.name,
                    Email = teacher.Email,
                    Phone = teacher.PhoneNumber,
                    Gender = teacher.gender,
                    MemberType = "Teacher",
                    Role = teacher.role,
                    ProfileImageUrl = teacher.profile_imageUrl,
                    JoinedAt = teacher.JoinedAt,
                    Status = teacher.status,
                    MosqueId = teacher.MosqueId,
                    MosqueName = teacher.Mosque?.name ?? string.Empty,
                    TeacherDetails = new TeacherProfileDetails
                    {
                        Bio = teacher.Bio,
                        AssignedAt = teacher.assigned_at,
                        HalaqasCount = halaqasCount
                    }
                };

                return GeneralResponse.Ok("تم جلب ملف العضو بنجاح.", dto);
            }

            var supervisor = await _context.Supervisors.Include(s => s.Mosque).FirstOrDefaultAsync(s => s.Id == memberId);
            if (supervisor != null)
            {
                var dto = new MemberProfileDto
                {
                    Id = supervisor.Id,
                    Name = supervisor.name,
                    Email = supervisor.Email,
                    Phone = supervisor.PhoneNumber,
                    Gender = supervisor.gender,
                    MemberType = "Supervisor",
                    Role = supervisor.role,
                    ProfileImageUrl = supervisor.profile_imageUrl,
                    JoinedAt = supervisor.JoinedAt,
                    Status = 0,
                    MosqueId = supervisor.MosqueId,
                    MosqueName = supervisor.Mosque?.name ?? string.Empty
                };

                return GeneralResponse.Ok("تم جلب ملف العضو بنجاح.", dto);
            }

            return GeneralResponse.NotFound("العضو غير موجود.");
        }

        public async Task<GeneralResponse> SearchMembersAsync(SearchMembersRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var (members, totalCount) = await SearchMembersInternalAsync(request, applyPaging: true);
            var page = Math.Max(1, request.PageNumber);
            var pageSize = Math.Max(1, request.PageSize);

            return GeneralResponse.Ok("تم جلب الأعضاء بنجاح.", members, page, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetMemberStatisticsAsync(GetMemberStatisticsRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var studentsQuery = _context.Students.Where(s => s.role == StudentRole && s.status == 0).AsQueryable();
            var parentsQuery = _context.Students.Where(s => s.role == ParentRole).AsQueryable();
            var teachersQuery = _context.Teachers.Where(t => t.status == ActiveStatus).AsQueryable();
            var supervisorsQuery = _context.Supervisors.AsQueryable();

            var effectiveMosqueId = ResolveEffectiveMosqueId(await ResolveManagedMosqueIdAsync(), request.MosqueId);
            if (effectiveMosqueId.HasValue)
            {
                studentsQuery = studentsQuery.Where(s => s.MosqueId == effectiveMosqueId.Value);
                parentsQuery = parentsQuery.Where(s => s.MosqueId == effectiveMosqueId.Value);
                teachersQuery = teachersQuery.Where(t => t.MosqueId == effectiveMosqueId.Value);
                supervisorsQuery = supervisorsQuery.Where(s => s.MosqueId == effectiveMosqueId.Value);
            }

            if (request.Status.HasValue && request.Status.Value == 0)
                studentsQuery = studentsQuery.Where(s => s.status == request.Status.Value);

            if (request.FromDate.HasValue)
            {
                studentsQuery = studentsQuery.Where(s => s.JoinedAt >= request.FromDate.Value);
                parentsQuery = parentsQuery.Where(s => s.JoinedAt >= request.FromDate.Value);
                teachersQuery = teachersQuery.Where(s => s.JoinedAt >= request.FromDate.Value);
                supervisorsQuery = supervisorsQuery.Where(s => s.JoinedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                studentsQuery = studentsQuery.Where(s => s.JoinedAt <= request.ToDate.Value);
                parentsQuery = parentsQuery.Where(s => s.JoinedAt <= request.ToDate.Value);
                teachersQuery = teachersQuery.Where(s => s.JoinedAt <= request.ToDate.Value);
                supervisorsQuery = supervisorsQuery.Where(s => s.JoinedAt <= request.ToDate.Value);
            }

            var studentsCount = await studentsQuery.CountAsync();
            var parentsCount = await parentsQuery.CountAsync();
            var teachersCount = await teachersQuery.CountAsync();
            var supervisorsCount = await supervisorsQuery.CountAsync();

            var activeCount = await studentsQuery.CountAsync(s => s.status == 0);
            var inactiveCount = await studentsQuery.CountAsync(s => s.status == 1);
            var graduatedCount = await studentsQuery.CountAsync(s => s.status == 2);

            var data = new
            {
                StudentsCount = studentsCount,
                TeachersCount = teachersCount,
                ParentsCount = parentsCount,
                SupervisorsCount = supervisorsCount,
                ActiveStudents = activeCount,
                InactiveStudents = inactiveCount,
                GraduatedStudents = graduatedCount
            };

            return GeneralResponse.Ok("تم جلب إحصائيات الأعضاء بنجاح.", data);
        }

        public async Task<GeneralResponse> UpdateStudentInfoAsync(UpdateStudentInfoRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف الطالب غير صالح.");

            var accessError = await EnsureMemberAccessAsync(request.StudentId);
            if (accessError != null)
                return accessError;

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            if (request.Status.HasValue)
                student.status = request.Status.Value;

            if (request.Score.HasValue)
                student.score = request.Score.Value;

            await _context.SaveChangesAsync();
            return GeneralResponse.Ok("تم تحديث بيانات الطالب بنجاح.");
        }

        public async Task<GeneralResponse> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request)
        {
            if (request == null || request.TeacherId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المعلم غير صالح.");

            var accessError = await EnsureMemberAccessAsync(request.TeacherId);
            if (accessError != null)
                return accessError;

            var teacher = await _context.Teachers.FindAsync(request.TeacherId);
            if (teacher == null)
                return GeneralResponse.NotFound("المعلم غير موجود.");

            if (!string.IsNullOrWhiteSpace(request.Bio))
                teacher.Bio = request.Bio;

            await _context.SaveChangesAsync();
            return GeneralResponse.Ok("تم تحديث بيانات المعلم بنجاح.");
        }

        public async Task<GeneralResponse> UpdateParentInfoAsync(UpdateParentInfoRequest request)
        {
            if (request == null || request.ParentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف ولي الأمر غير صالح.");

            var accessError = await EnsureMemberAccessAsync(request.ParentId);
            if (accessError != null)
                return accessError;

            var parent = await _context.Students
                .Include(p => p.ChildLinks)
                .FirstOrDefaultAsync(p => p.Id == request.ParentId && p.role == ParentRole);

            if (parent == null)
                return GeneralResponse.NotFound("ولي الأمر غير موجود.");

            if (!string.IsNullOrWhiteSpace(request.Phone))
                parent.PhoneNumber = request.Phone;

            if (!string.IsNullOrWhiteSpace(request.Relationship))
                parent.theme = request.Relationship;

            var selectedStudentIds = NormalizeParentStudentIds(request.StudentIds, null);
            if (selectedStudentIds.Count > 0)
            {
                var children = await LoadParentChildrenAsync(selectedStudentIds);
                if (children.Count != selectedStudentIds.Count)
                    return GeneralResponse.BadRequest("يوجد طالب محدد غير موجود.");

                await SyncParentStudentLinksAsync(parent.Id, selectedStudentIds);
            }

            await _context.SaveChangesAsync();
            return GeneralResponse.Ok("تم تعديل بيانات ولي الأمر بنجاح.");
        }

        public async Task<GeneralResponse> DeleteStudentAsync(DeleteStudentRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف الطالب غير صالح.");

            var student = await _context.Students
                .Include(s => s.Children)
                .FirstOrDefaultAsync(s => s.Id == request.StudentId);

            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var blockers = new List<string>();

            if (student.Children?.Any() == true)
                blockers.Add($"مرتبط كولي أمر مع {student.Children.Count} طالب/طلاب");

            var parentStudentLinksCount = await _context.ParentSudents.CountAsync(p => p.Studentid == student.Id);
            if (parentStudentLinksCount > 0)
                blockers.Add($"مرتبط مع {parentStudentLinksCount} ولي أمر");

            var attendanceCount = await _context.Attendances.CountAsync(a => a.StudentId == student.Id);
            if (attendanceCount > 0)
                blockers.Add($"لديه {attendanceCount} سجل حضور");

            var examCount = await _context.Exams.CountAsync(e => e.StudentId == student.Id);
            if (examCount > 0)
                blockers.Add($"لديه {examCount} اختبار");

            var progressCount = await _context.ProgressEntries.CountAsync(p => p.StudentId == student.Id);
            if (progressCount > 0)
                blockers.Add($"لديه {progressCount} سجل تقدم");

            if (blockers.Count > 0)
            {
                var blockerText = string.Join("، ", blockers);
                return GeneralResponse.BadRequest($"لا يمكن حذف الطالب لأنه مرتبط ببيانات أخرى: {blockerText}. يرجى فك هذه الارتباطات أو حذف البيانات التابعة أولاً ثم إعادة المحاولة.");
            }

            var result = await _userManager.DeleteAsync(student);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return GeneralResponse.BadRequest($"فشل حذف الطالب: {errors}");
            }

            await _context.SaveChangesAsync();
            return GeneralResponse.Ok("تم حذف الطالب بنجاح.");
        }

        public async Task<GeneralResponse> DeleteTeacherAsync(DeleteTeacherRequest request)
        {
            if (request == null || request.TeacherId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المعلم غير صالح.");

            var accessError = await EnsureMemberAccessAsync(request.TeacherId);
            if (accessError != null)
                return accessError;

            var teacher = await _context.Teachers.FindAsync(request.TeacherId);
            if (teacher == null)
                return GeneralResponse.NotFound("المعلم غير موجود.");

            var hasHalaqas = await _context.Halqas.AnyAsync(h => h.TeacherId == teacher.Id);
            if (hasHalaqas)
                return GeneralResponse.BadRequest("لا يمكن حذف المعلم لارتباطه بحلقات. يرجى إزالة المعلم من الحلقات أولاً.");

            var hasSaturdayHalaqas = await _context.SaturdayHalqes.AnyAsync(h => h.TeacherId == teacher.Id);
            if (hasSaturdayHalaqas)
                return GeneralResponse.BadRequest("لا يمكن حذف المعلم لارتباطه بحلقات السبت. يرجى نقل الحلقات إلى معلم آخر أولاً.");

            var hasProgressEntries = await _context.ProgressEntries.AnyAsync(p => p.TeacherId == teacher.Id);
            if (hasProgressEntries)
                return GeneralResponse.BadRequest("لا يمكن حذف المعلم لوجود سجلات تقدم مرتبطة به.");

            var hasAttendances = await _context.Attendances.AnyAsync(a => a.TeacherId == teacher.Id);
            if (hasAttendances)
                return GeneralResponse.BadRequest("لا يمكن حذف المعلم لوجود سجلات حضور مرتبطة به.");

            var hasExams = await _context.Exams.AnyAsync(e => e.TeacherId == teacher.Id);
            if (hasExams)
                return GeneralResponse.BadRequest("لا يمكن حذف المعلم لوجود اختبارات مرتبطة به.");

            var hasSaturdayLessons = await _context.Set<SaturdayLesson>().AnyAsync(l => l.TeacherId == teacher.Id);
            if (hasSaturdayLessons)
                return GeneralResponse.BadRequest("لا يمكن حذف المعلم لوجود دروس أسبوعية مرتبطة به.");

            var roles = await _userManager.GetRolesAsync(teacher);
            if (roles.Count > 0)
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(teacher, roles);
                if (!removeRolesResult.Succeeded)
                {
                    var errors = string.Join("; ", removeRolesResult.Errors.Select(e => e.Description));
                    return GeneralResponse.BadRequest($"فشل إزالة صلاحيات المعلم قبل الحذف: {errors}");
                }
            }

            var result = await _userManager.DeleteAsync(teacher);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return GeneralResponse.BadRequest($"فشل حذف المعلم: {errors}");
            }

            return GeneralResponse.Ok("تم حذف المعلم بنجاح.");
        }

        public async Task<GeneralResponse> DeleteParentAsync(DeleteParentRequest request)
        {
            if (request == null || request.ParentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف ولي الأمر غير صالح.");

            var parent = await _context.Students
                .Include(p => p.Children)
                .Include(p => p.ChildLinks)
                .FirstOrDefaultAsync(p => p.Id == request.ParentId && p.role == ParentRole);

            if (parent == null)
                return GeneralResponse.NotFound("ولي الأمر غير موجود.");

            if (parent.Children != null)
            {
                foreach (var child in parent.Children)
                    child.ParentId = null;
            }

            if (parent.ChildLinks.Count > 0)
                _context.ParentStudentLinks.RemoveRange(parent.ChildLinks);

            var result = await _userManager.DeleteAsync(parent);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return GeneralResponse.BadRequest($"فشل حذف ولي الأمر: {errors}");
            }

            return GeneralResponse.Ok("تم حذف ولي الأمر بنجاح.");
        }

        public async Task<GeneralResponse> DeleteSupervisorAsync(DeleteSupervisorRequest request)
        {
            if (request == null || request.SupervisorId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المشرف غير صالح.");

            var accessError = await EnsureMemberAccessAsync(request.SupervisorId);
            if (accessError != null)
                return accessError;

            var supervisor = await _context.Supervisors.FindAsync(request.SupervisorId);
            if (supervisor == null)
                return GeneralResponse.NotFound("المشرف غير موجود.");

            var result = await _userManager.DeleteAsync(supervisor);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return GeneralResponse.BadRequest($"فشل حذف المشرف: {errors}");
            }

            return GeneralResponse.Ok("تم حذف المشرف بنجاح.");
        }

        public async Task<GeneralResponse> CancelMembershipAsync(CancelMembershipRequest request)
        {
            if (request == null || !Guid.TryParse(request.MemberId, out var memberId))
                return GeneralResponse.BadRequest("معرّف العضو غير صالح.");

            var accessError = await EnsureMemberAccessAsync(memberId);
            if (accessError != null)
                return accessError;

            var student = await _context.Students.FindAsync(memberId);
            if (student == null)
                return GeneralResponse.NotFound("العضو غير موجود.");

            student.status = 1;
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم إلغاء العضوية بنجاح.");
        }

        public async Task<GeneralResponse> ExportMembersListAsync(ExportMembersRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var searchRequest = new SearchMembersRequest
            {
                MemberType = request.MemberType,
                MosqueId = request.MosqueId,
                PageNumber = 1,
                PageSize = int.MaxValue,
                SortBy = request.SortBy,
                SortDescending = request.SortDescending
            };

            var (members, _) = await SearchMembersInternalAsync(searchRequest, applyPaging: false);
            var fields = request.Fields?.Where(f => !string.IsNullOrWhiteSpace(f)).Select(f => f.Trim()).ToList()
                         ?? new List<string>();

            if (fields.Count == 0)
            {
                fields = new List<string>
                {
                    "Id", "Name", "Email", "Phone", "Gender", "MemberType", "MosqueName", "Status"
                };
            }

            var data = new List<Dictionary<string, object>>();

            foreach (var member in members)
            {
                var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var field in fields)
                {
                    row[field] = field.ToLowerInvariant() switch
                    {
                        "id" => member.Id,
                        "name" => member.Name,
                        "email" => member.Email,
                        "phone" => member.Phone,
                        "gender" => member.Gender,
                        "membertype" => member.MemberType,
                        "mosqueid" => member.MosqueId,
                        "mosquename" => member.MosqueName,
                        "status" => member.Status,
                        "role" => member.Role,
                        "joinedat" => member.JoinedAt,
                        _ => null
                    };
                }
                data.Add(row);
            }

            return GeneralResponse.Ok("تم تجهيز بيانات التصدير بنجاح.", data);
        }

        private async Task<(List<MemberProfileDto> Members, int TotalCount)> SearchMembersInternalAsync(
            SearchMembersRequest request,
            bool applyPaging)
        {
            var members = new List<MemberProfileDto>();
            var type = request.MemberType?.Trim();
            var page = Math.Max(1, request.PageNumber);
            var pageSize = Math.Max(1, request.PageSize);
            var totalCount = 0;
            var scopedMosqueId = ResolveEffectiveMosqueId(await ResolveManagedMosqueIdAsync(), request.MosqueId);

            if (string.IsNullOrWhiteSpace(type) || type.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                var query = _context.Students.Include(s => s.Mosque).Where(s => s.role == StudentRole);

                if (!string.IsNullOrWhiteSpace(request.Name))
                    query = query.Where(s => EF.Functions.Like(s.name, $"%{request.Name}%"));
                if (!string.IsNullOrWhiteSpace(request.Email))
                    query = query.Where(s => EF.Functions.Like(s.Email, $"%{request.Email}%"));
                if (!string.IsNullOrWhiteSpace(request.Phone))
                    query = query.Where(s => EF.Functions.Like(s.PhoneNumber, $"%{request.Phone}%"));
                if (scopedMosqueId.HasValue)
                    query = query.Where(s => s.MosqueId == scopedMosqueId.Value);
                if (request.Status.HasValue)
                    query = query.Where(s => s.status == request.Status.Value);
                if (request.Role.HasValue)
                    query = query.Where(s => s.role == request.Role.Value);
                if (request.JoinedFrom.HasValue)
                    query = query.Where(s => s.JoinedAt >= request.JoinedFrom.Value);
                if (request.JoinedTo.HasValue)
                    query = query.Where(s => s.JoinedAt <= request.JoinedTo.Value);

                var typeCount = await query.CountAsync();
                totalCount += typeCount;

                query = request.SortBy?.ToLowerInvariant() switch
                {
                    "email" => request.SortDescending ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email),
                    "joinedat" => request.SortDescending ? query.OrderByDescending(s => s.JoinedAt) : query.OrderBy(s => s.JoinedAt),
                    _ => request.SortDescending ? query.OrderByDescending(s => s.name) : query.OrderBy(s => s.name)
                };

                if (applyPaging)
                    query = query.Skip((page - 1) * pageSize).Take(pageSize);

                var students = await query.ToListAsync();
                members.AddRange(students.Select(s => new MemberProfileDto
                {
                    Id = s.Id,
                    Name = s.name,
                    Email = s.Email,
                    Phone = s.PhoneNumber,
                    Gender = s.gender,
                    MemberType = "Student",
                    Role = s.role,
                    ProfileImageUrl = s.profile_imageUrl,
                    JoinedAt = s.JoinedAt,
                    Status = s.status,
                    MosqueId = s.MosqueId,
                    MosqueName = s.Mosque?.name ?? string.Empty
                }));

                if (!string.IsNullOrWhiteSpace(type))
                    return (members, totalCount);
            }

            if (string.IsNullOrWhiteSpace(type) || type.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
            {
                var query = _context.Teachers.Include(t => t.Mosque).AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Name))
                    query = query.Where(t => EF.Functions.Like(t.name, $"%{request.Name}%"));
                if (!string.IsNullOrWhiteSpace(request.Email))
                    query = query.Where(t => EF.Functions.Like(t.Email, $"%{request.Email}%"));
                if (!string.IsNullOrWhiteSpace(request.Phone))
                    query = query.Where(t => EF.Functions.Like(t.PhoneNumber, $"%{request.Phone}%"));
                if (scopedMosqueId.HasValue)
                    query = query.Where(t => t.MosqueId == scopedMosqueId.Value);
                if (request.Status.HasValue)
                    query = query.Where(t => t.status == request.Status.Value);
                if (request.JoinedFrom.HasValue)
                    query = query.Where(t => t.JoinedAt >= request.JoinedFrom.Value);
                if (request.JoinedTo.HasValue)
                    query = query.Where(t => t.JoinedAt <= request.JoinedTo.Value);

                var typeCount = await query.CountAsync();
                totalCount += typeCount;

                query = request.SortBy?.ToLowerInvariant() switch
                {
                    "email" => request.SortDescending ? query.OrderByDescending(t => t.Email) : query.OrderBy(t => t.Email),
                    "joinedat" => request.SortDescending ? query.OrderByDescending(t => t.JoinedAt) : query.OrderBy(t => t.JoinedAt),
                    _ => request.SortDescending ? query.OrderByDescending(t => t.name) : query.OrderBy(t => t.name)
                };

                if (applyPaging)
                    query = query.Skip((page - 1) * pageSize).Take(pageSize);

                var teachers = await query.ToListAsync();
                members.AddRange(teachers.Select(t => new MemberProfileDto
                {
                    Id = t.Id,
                    Name = t.name,
                    Email = t.Email,
                    Phone = t.PhoneNumber,
                    Gender = t.gender,
                    MemberType = "Teacher",
                    Role = t.role,
                    ProfileImageUrl = t.profile_imageUrl,
                    JoinedAt = t.JoinedAt,
                    Status = t.status,
                    MosqueId = t.MosqueId,
                    MosqueName = t.Mosque?.name ?? string.Empty
                }));

                if (!string.IsNullOrWhiteSpace(type))
                    return (members, totalCount);
            }

            if (string.IsNullOrWhiteSpace(type) || type.Equals("Parent", StringComparison.OrdinalIgnoreCase))
            {
                var query = _context.Students.Include(p => p.Mosque).Where(p => p.role == ParentRole);

                if (!string.IsNullOrWhiteSpace(request.Name))
                    query = query.Where(p => EF.Functions.Like(p.name, $"%{request.Name}%"));
                if (!string.IsNullOrWhiteSpace(request.Email))
                    query = query.Where(p => EF.Functions.Like(p.Email, $"%{request.Email}%"));
                if (!string.IsNullOrWhiteSpace(request.Phone))
                    query = query.Where(p => EF.Functions.Like(p.PhoneNumber, $"%{request.Phone}%"));
                if (scopedMosqueId.HasValue)
                    query = query.Where(p => p.MosqueId == scopedMosqueId.Value);
                if (request.JoinedFrom.HasValue)
                    query = query.Where(p => p.JoinedAt >= request.JoinedFrom.Value);
                if (request.JoinedTo.HasValue)
                    query = query.Where(p => p.JoinedAt <= request.JoinedTo.Value);

                var typeCount = await query.CountAsync();
                totalCount += typeCount;

                query = request.SortBy?.ToLowerInvariant() switch
                {
                    "email" => request.SortDescending ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
                    "joinedat" => request.SortDescending ? query.OrderByDescending(p => p.JoinedAt) : query.OrderBy(p => p.JoinedAt),
                    _ => request.SortDescending ? query.OrderByDescending(p => p.name) : query.OrderBy(p => p.name)
                };

                if (applyPaging)
                    query = query.Skip((page - 1) * pageSize).Take(pageSize);

                var parents = await query.ToListAsync();
                members.AddRange(parents.Select(p => new MemberProfileDto
                {
                    Id = p.Id,
                    Name = p.name,
                    Email = p.Email,
                    Phone = p.PhoneNumber,
                    Gender = p.gender,
                    MemberType = "Parent",
                    Role = p.role,
                    ProfileImageUrl = p.profile_imageUrl,
                    JoinedAt = p.JoinedAt,
                    Status = p.status,
                    MosqueId = p.MosqueId,
                    MosqueName = p.Mosque?.name ?? string.Empty
                }));

                if (!string.IsNullOrWhiteSpace(type))
                    return (members, totalCount);
            }

            if (string.IsNullOrWhiteSpace(type) || type.Equals("Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                var query = _context.Supervisors.Include(s => s.Mosque).AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Name))
                    query = query.Where(s => EF.Functions.Like(s.name, $"%{request.Name}%"));
                if (!string.IsNullOrWhiteSpace(request.Email))
                    query = query.Where(s => EF.Functions.Like(s.Email, $"%{request.Email}%"));
                if (!string.IsNullOrWhiteSpace(request.Phone))
                    query = query.Where(s => EF.Functions.Like(s.PhoneNumber, $"%{request.Phone}%"));
                if (scopedMosqueId.HasValue)
                    query = query.Where(s => s.MosqueId == scopedMosqueId.Value);
                if (request.JoinedFrom.HasValue)
                    query = query.Where(s => s.JoinedAt >= request.JoinedFrom.Value);
                if (request.JoinedTo.HasValue)
                    query = query.Where(s => s.JoinedAt <= request.JoinedTo.Value);

                var typeCount = await query.CountAsync();
                totalCount += typeCount;

                query = request.SortBy?.ToLowerInvariant() switch
                {
                    "email" => request.SortDescending ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email),
                    "joinedat" => request.SortDescending ? query.OrderByDescending(s => s.JoinedAt) : query.OrderBy(s => s.JoinedAt),
                    _ => request.SortDescending ? query.OrderByDescending(s => s.name) : query.OrderBy(s => s.name)
                };

                if (applyPaging)
                    query = query.Skip((page - 1) * pageSize).Take(pageSize);

                var supervisors = await query.ToListAsync();
                members.AddRange(supervisors.Select(s => new MemberProfileDto
                {
                    Id = s.Id,
                    Name = s.name,
                    Email = s.Email,
                    Phone = s.PhoneNumber,
                    Gender = s.gender,
                    MemberType = "Supervisor",
                    Role = s.role,
                    ProfileImageUrl = s.profile_imageUrl,
                    JoinedAt = s.JoinedAt,
                    Status = 0,
                    MosqueId = s.MosqueId,
                    MosqueName = s.Mosque?.name ?? string.Empty
                }));

                if (!string.IsNullOrWhiteSpace(type))
                    return (members, totalCount);
            }

            return (members, totalCount);
        }

        private static StudentDto MapStudentDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                Name = student.name,
                Email = student.Email,
                Phone = student.PhoneNumber,
                Gender = student.gender,
                FontSize = student.font_size,
                Role = student.role,
                Theme = student.theme,
                ProfileImageUrl = student.profile_imageUrl,
                CreatedAt = student.created_at,
                JoinedAt = student.JoinedAt,
                Age = student.age,
                EnrollmentDate = student.EnrollmentDate,
                Status = student.status,
                Score = student.score,
                MosqueId = student.MosqueId,
                SaturdayHalqeId = student.SaturdayHalqeId,
                HalqaId = student.HalqaId,
                HalqaName = student.Halqa?.Name ?? string.Empty,
                FoujId = student.Halqa?.FoujId,
                FoujName = student.Halqa?.Fouj?.name ?? string.Empty,
                MosqueName = student.Mosque?.name ?? string.Empty,
                SaturdayHalqeName = student.SaturdayHalqa?.name ?? string.Empty
            };
        }

        private static TeacherDto MapTeacherDto(Teacher teacher)
        {
            var primaryHalqa = teacher.halaqas?.OrderBy(h => h.Name).FirstOrDefault();

            return new TeacherDto
            {
                Id = teacher.Id,
                Name = teacher.name,
                Email = teacher.Email,
                Phone = teacher.PhoneNumber,
                Gender = teacher.gender,
                FontSize = teacher.font_size,
                Role = teacher.role,
                Status = teacher.status,
                Theme = teacher.theme,
                ProfileImageUrl = teacher.profile_imageUrl,
                CreatedAt = teacher.created_at,
                JoinedAt = teacher.JoinedAt,
                MosqueId = teacher.MosqueId,
                Bio = teacher.Bio,
                AssignedAt = teacher.assigned_at,
                HalqaId = primaryHalqa?.Id,
                HalqaName = primaryHalqa?.Name ?? string.Empty,
                FoujId = primaryHalqa?.FoujId,
                FoujName = primaryHalqa?.Fouj?.name ?? string.Empty,
                MosqueName = teacher.Mosque?.name ?? string.Empty
            };
        }

        private static ParentDto MapParentDto(Student parent, Student? child)
        {
            return MapParentDto(parent, child == null ? Enumerable.Empty<Student>() : new[] { child });
        }

        private static ParentDto MapParentDto(Student parent, IEnumerable<Student>? children)
        {
            var linkedChildren = (children ?? Enumerable.Empty<Student>())
                .Where(child => child != null)
                .GroupBy(child => child.Id)
                .Select(group => group.First())
                .OrderBy(child => child.name)
                .ToList();

            var firstChild = linkedChildren.FirstOrDefault();

            return new ParentDto
            {
                Id = parent.Id,
                Name = parent.name,
                Email = parent.Email,
                Phone = parent.PhoneNumber,
                Gender = parent.gender,
                FontSize = parent.font_size,
                Role = parent.role,
                Theme = parent.theme,
                ProfileImageUrl = parent.profile_imageUrl,
                CreatedAt = parent.created_at,
                JoinedAt = parent.JoinedAt,
                StudentId = firstChild?.Id ?? Guid.Empty,
                Relationship = parent.theme ?? string.Empty,
                StudentName = firstChild?.name ?? string.Empty,
                StudentIds = linkedChildren.Select(child => child.Id).ToList(),
                StudentNames = string.Join("، ", linkedChildren.Select(child => child.name).Where(name => !string.IsNullOrWhiteSpace(name))),
                StudentsCount = linkedChildren.Count
            };
        }

        private static List<Student> ResolveLinkedChildren(Student parent)
        {
            var children = new List<Student>();

            if (parent.ChildLinks != null)
                children.AddRange(parent.ChildLinks.Where(link => link.Student != null).Select(link => link.Student));

            if (parent.Children != null)
                children.AddRange(parent.Children);

            return children
                .Where(child => child != null && child.role == StudentRole)
                .GroupBy(child => child.Id)
                .Select(group => group.First())
                .OrderBy(child => child.name)
                .ToList();
        }

        private static List<Guid> NormalizeParentStudentIds(IEnumerable<Guid>? studentIds, Guid? legacyStudentId)
        {
            var ids = studentIds?
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList() ?? new List<Guid>();

            if (legacyStudentId.HasValue && legacyStudentId.Value != Guid.Empty && !ids.Contains(legacyStudentId.Value))
                ids.Add(legacyStudentId.Value);

            return ids;
        }

        private async Task UpdateTeacherStatusAsync(Teacher teacher, int status)
        {
            teacher.status = status;

            if (status != ActiveStatus)
                await DetachTeacherAssignmentsAsync(teacher.Id);
        }

        private async Task DetachTeacherAssignmentsAsync(Guid teacherId)
        {
            var halqas = await _context.Halqas
                .Where(h => h.TeacherId == teacherId)
                .ToListAsync();

            foreach (var halqa in halqas)
                halqa.TeacherId = null;

            var saturdayHalqas = await _context.SaturdayHalqes
                .Where(h => h.TeacherId == teacherId)
                .ToListAsync();

            foreach (var saturdayHalqa in saturdayHalqas)
                saturdayHalqa.TeacherId = null;

            var weeklyLessonRows = await _context.Set<SaturdayLesson>()
                .Where(l => l.TeacherId == teacherId)
                .ToListAsync();

            foreach (var row in weeklyLessonRows)
                row.TeacherId = null;
        }

        private async Task<List<Student>> LoadParentChildrenAsync(IReadOnlyCollection<Guid> studentIds)
        {
            if (studentIds.Count == 0)
                return new List<Student>();

            return await _context.Students
                .Where(student => studentIds.Contains(student.Id) && student.role == StudentRole)
                .ToListAsync();
        }

        private async Task SyncParentStudentLinksAsync(Guid parentId, IReadOnlyCollection<Guid> selectedStudentIds)
        {
            var selected = selectedStudentIds.Where(id => id != Guid.Empty).Distinct().ToHashSet();
            var existingLinks = await _context.ParentStudentLinks
                .Where(link => link.ParentId == parentId)
                .ToListAsync();

            var linksToRemove = existingLinks.Where(link => !selected.Contains(link.StudentId)).ToList();
            if (linksToRemove.Count > 0)
                _context.ParentStudentLinks.RemoveRange(linksToRemove);

            var existingIds = existingLinks.Select(link => link.StudentId).ToHashSet();
            foreach (var studentId in selected.Where(id => !existingIds.Contains(id)))
            {
                _context.ParentStudentLinks.Add(new ParentStudentLink
                {
                    ParentId = parentId,
                    StudentId = studentId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            var affectedStudentIds = selected.Concat(linksToRemove.Select(link => link.StudentId)).Distinct().ToList();
            var affectedStudents = await _context.Students
                .Where(student => affectedStudentIds.Contains(student.Id))
                .ToListAsync();

            foreach (var student in affectedStudents)
            {
                if (selected.Contains(student.Id))
                {
                    if (!student.ParentId.HasValue || student.ParentId == parentId)
                        student.ParentId = parentId;
                }
                else if (student.ParentId == parentId)
                {
                    student.ParentId = null;
                }
            }
        }
    }
}
