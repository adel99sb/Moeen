using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;

namespace Moeen.Api.Infrastructure.Data
{
    public static class AppSeeder
    {
        private static readonly Guid DevelopmentMosqueId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid DevelopmentSupervisorId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid DevelopmentTeacherId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public const string DevelopmentSupervisorEmail = "supervisor@moeen.local";
        public const string DevelopmentTeacherEmail = "teacher@moeen.local";
        private const string DevelopmentSupervisorPasswordConfigurationKey = "SeedUsers:DevelopmentSupervisorPassword";

        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = role,
                        NormalizedName = role.ToUpperInvariant()
                    });
                }
            }
        }

        public static async Task SeedDevelopmentDataAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var developmentSupervisorPassword = "MoeenDevOnly123!";

            if (string.IsNullOrWhiteSpace(developmentSupervisorPassword))
            {
                throw new InvalidOperationException($"Missing required development seed setting: {DevelopmentSupervisorPasswordConfigurationKey}");
            }

            await SeedRolesAsync(roleManager);
            await SeedDevelopmentMosqueAsync(context);
            await SeedDevelopmentSupervisorAsync(context, userManager, developmentSupervisorPassword);
            await SeedDevelopmentTeacherAsync(context, userManager, developmentSupervisorPassword);
        }

        private static async Task SeedDevelopmentMosqueAsync(AppDbContext context)
        {
            var mosque = await context.Mosques.FirstOrDefaultAsync(x => x.Id == DevelopmentMosqueId);

            if (mosque == null)
            {
                mosque = new Mosque
                {
                    Id = DevelopmentMosqueId,
                    name = "مسجد معين التجريبي",
                    address = "عنوان تجريبي لاختبار لوحة التحكم",
                    contact_phone = "0000000000",
                    Description = "مسجد seed محلي لاختبار المشرف والمنشورات.",
                    Latitude = 0,
                    Longitude = 0,
                    foujs = new List<Fouj>(),
                    Teachers = new List<Teacher>(),
                    TeacherExams = new List<TeacherExam>(),
                    pdfFiles = new List<PdfFile>(),
                    SaturdayLessons = new List<SaturdayLesson>(),
                    posts = new List<Post>(),
                    supervisors = new List<Supervisor>(),
                    Students = new List<Student>()
                };
                await context.Mosques.AddAsync(mosque);
            }
            else
            {
                mosque.name = "مسجد معين التجريبي";
                mosque.address = "عنوان تجريبي لاختبار لوحة التحكم";
                mosque.contact_phone = "0000000000";
                mosque.Description = "مسجد seed محلي لاختبار المشرف والمنشورات.";
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedDevelopmentTeacherAsync(AppDbContext context, UserManager<User> userManager, string developmentPassword)
        {
            var teacher = await context.Teachers.FirstOrDefaultAsync(x => x.Id == DevelopmentTeacherId);
            var existingTeacherUser = await userManager.FindByIdAsync(DevelopmentTeacherId.ToString())
                                      ?? await userManager.FindByEmailAsync(DevelopmentTeacherEmail);

            if (teacher == null && existingTeacherUser == null)
            {
                teacher = new Teacher
                {
                    Id = DevelopmentTeacherId,
                    UserName = DevelopmentTeacherEmail,
                    Email = DevelopmentTeacherEmail,
                    EmailConfirmed = true,
                    name = "معلم معين التجريبي",
                    gender = "Male",
                    font_size = 16,
                    role = (int)Roles.Teacher,
                    theme = "light",
                    profile_imageUrl = null,
                    created_at = DateTime.UtcNow,
                    JoinedAt = DateTime.UtcNow,
                    MosqueId = DevelopmentMosqueId,
                    Bio = "معلم seed محلي لاختبار لوحة المعلم والترقية.",
                    assigned_at = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    complaints = new List<Complaint>(),
                    PosInteractions = new List<PosInteraction>(),
                    halaqas = new List<Halqa>(),
                    ProgressEntrys = new List<ProgressEntry>(),
                    Attendances = new List<Attendance>()
                };

                var result = await userManager.CreateAsync(teacher, developmentPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed development teacher: {errors}");
                }
            }
            else
            {
                var userToUpdate = (User?)teacher ?? existingTeacherUser;
                if (userToUpdate == null)
                    throw new InvalidOperationException("Development teacher seed is in an invalid state.");

                userToUpdate.UserName = DevelopmentTeacherEmail;
                userToUpdate.Email = DevelopmentTeacherEmail;
                userToUpdate.EmailConfirmed = true;
                userToUpdate.role = (int)Roles.Teacher;

                if (teacher != null)
                {
                    teacher.name = "معلم معين التجريبي";
                    teacher.MosqueId = DevelopmentMosqueId;
                    teacher.Bio = string.IsNullOrWhiteSpace(teacher.Bio) ? "معلم seed محلي لاختبار لوحة المعلم والترقية." : teacher.Bio;
                    teacher.assigned_at = string.IsNullOrWhiteSpace(teacher.assigned_at) ? DateTime.UtcNow.ToString("yyyy-MM-dd") : teacher.assigned_at;
                    await context.SaveChangesAsync();
                }
                else
                {
                    userToUpdate.name = "معلم معين التجريبي";
                    var updateResult = await userManager.UpdateAsync(userToUpdate);
                    if (!updateResult.Succeeded)
                    {
                        var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to update development teacher user: {errors}");
                    }
                }
            }

            var teacherIdentityUser = (User?)teacher ?? existingTeacherUser;
            if (teacherIdentityUser == null)
                throw new InvalidOperationException("Development teacher user was not created.");

            await EnsureDevelopmentUserHasOnlyRoleAsync(userManager, teacherIdentityUser, Roles.Teacher, "development teacher");
        }

        private static async Task SeedDevelopmentSupervisorAsync(AppDbContext context, UserManager<User> userManager, string developmentSupervisorPassword)
        {
            var supervisor = await context.Supervisors.FirstOrDefaultAsync(x => x.Id == DevelopmentSupervisorId);

            if (supervisor == null)
            {
                supervisor = new Supervisor
                {
                    Id = DevelopmentSupervisorId,
                    UserName = DevelopmentSupervisorEmail,
                    Email = DevelopmentSupervisorEmail,
                    EmailConfirmed = true,
                    name = "مشرف معين التجريبي",
                    gender = "Male",
                    font_size = 16,
                    role = (int)Roles.Admin,
                    theme = "light",
                    profile_imageUrl = null,
                    created_at = DateTime.UtcNow,
                    JoinedAt = DateTime.UtcNow,
                    MosqueId = DevelopmentMosqueId,
                    assigned_at = DateTime.UtcNow,
                    complaints = new List<Complaint>(),
                    PosInteractions = new List<PosInteraction>()
                };

                var result = await userManager.CreateAsync(supervisor, developmentSupervisorPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed development supervisor: {errors}");
                }
            }
            else
            {
                supervisor.UserName = DevelopmentSupervisorEmail;
                supervisor.Email = DevelopmentSupervisorEmail;
                supervisor.EmailConfirmed = true;
                supervisor.name = "مشرف معين التجريبي";
                supervisor.role = (int)Roles.Admin;
                supervisor.MosqueId = DevelopmentMosqueId;
                supervisor.assigned_at = supervisor.assigned_at == default ? DateTime.UtcNow : supervisor.assigned_at;
                await context.SaveChangesAsync();
            }

            if (!await userManager.CheckPasswordAsync(supervisor, developmentSupervisorPassword))
            {
                if (await userManager.HasPasswordAsync(supervisor))
                {
                    var removePasswordResult = await userManager.RemovePasswordAsync(supervisor);
                    if (!removePasswordResult.Succeeded)
                    {
                        var errors = string.Join(", ", removePasswordResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to remove development supervisor password: {errors}");
                    }
                }

                var addPasswordResult = await userManager.AddPasswordAsync(supervisor, developmentSupervisorPassword);
                if (!addPasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to set development supervisor password: {errors}");
                }
            }

            await EnsureDevelopmentUserHasOnlyRoleAsync(userManager, supervisor, Roles.Admin, "development supervisor");
        }
        private static async Task EnsureDevelopmentUserHasOnlyRoleAsync(
            UserManager<User> userManager,
            User user,
            Roles desiredRole,
            string seedName)
        {
            var desiredRoleName = desiredRole.ToString();
            var currentRoles = await userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles
                .Where(role => !string.Equals(role, desiredRoleName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (rolesToRemove.Length > 0)
            {
                var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to remove stale roles from {seedName}: {errors}");
                }
            }

            if (!currentRoles.Any(role => string.Equals(role, desiredRoleName, StringComparison.OrdinalIgnoreCase)))
            {
                var roleResult = await userManager.AddToRoleAsync(user, desiredRoleName);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to assign {seedName} role: {errors}");
                }
            }
        }

    }
}
