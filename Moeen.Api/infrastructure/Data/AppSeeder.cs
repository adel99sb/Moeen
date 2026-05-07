using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;

namespace Moeen.Api.Infrastructure.Data
{
    public static class AppSeeder
    {
        // ================= [الدالة القديمة - ما تغير شي] =================
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
                        NormalizedName = role.ToUpper()
                    });
                }
            }
        }

        // ================= [الدالة الجديدة الكاملة] =================
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            // 1. الحصول على الخدمات اللازمة
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // تأكد إن اسم الكونتكست صحيح عندك
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // 2. إنشاء مسجد وهمي (ضروري عشان الـ Foreign Key ما يضلّع)
            var mosqueId = Guid.NewGuid();
            var mosque = new Mosque
            {
                Id = mosqueId,
                name = "المسجد الكبير", // تأكد من اسم الخاصية في كلاس Mosque
                // أي خصائص تانية مطلوبة للـ Mosque حطها هنا
            };
            await context.Mosques.AddAsync(mosque);
            await context.SaveChangesAsync(); // حفظ عشان ينولد الـ ID

            // 3. إنشاء حلقة يوم سبت (ضرورية للطالب)
            var halqaId = Guid.NewGuid();
            var saturdayHalqa = new SaturdayHalqa
            {
                Id = halqaId,
                MosqueId = mosqueId, // ربط الحلقة بالمسجد
                // أي خصائص تانية مطلوبة
            };
            await context.Set<SaturdayHalqa>().AddAsync(saturdayHalqa);
            await context.SaveChangesAsync();

            // 4. إنشاء المستخدمين
            // ---------------- أ) المعلم (Teacher) ----------------
            var teacher = new Teacher
            {
                Id = Guid.NewGuid(),
                UserName = "teacher@moeen.com",
                Email = "teacher@moeen.com",
                name = "الأستاذ أحمد",
                gender = "Male",
                MosqueId = mosqueId, // ✅ استخدام الـ ID الحقيقي
                Bio = "معلم قرآن كريم",
                assigned_at = DateTime.UtcNow.ToString(),
                created_at = DateTime.UtcNow,
                JoinedAt = DateTime.UtcNow,
                // تهيئة القوائم لتجنب الأخطاء
                halaqas = new List<Halqa>(),
                ProgressEntrys = new List<ProgressEntry>(),
                Attendances = new List<Attendance>()
            };

            // ---------------- ب) الطالب (Student) ----------------
            var student = new Student
            {
                Id = Guid.NewGuid(),
                UserName = "student@moeen.com",
                Email = "student@moeen.com",
                name = "الطفل محمد",
                gender = "Male", // ✅ Student.gender مطلوب وليس اختياري
                age = 10,
                MosqueId = mosqueId, // ✅ استخدام الـ ID الحقيقي
                SaturdayHalqeId = halqaId, // ✅ استخدام الـ ID الحقيقي
                EnrollmentDate = DateTime.UtcNow,
                status = 1,
                score = 85,
                created_at = DateTime.UtcNow,
                JoinedAt = DateTime.UtcNow,
                progressEntrys = new List<ProgressEntry>(),
                Exams = new List<Exam>(),
                Attendances = new List<Attendance>(),
                Children = new List<Student>()
            };

            // ---------------- ج) ولي الأمر (Parent) ----------------
            var parent = new Student // ولي الأمر هو أيضاً Student
            {
                Id = Guid.NewGuid(),
                UserName = "parent@moeen.com",
                Email = "parent@moeen.com",
                name = "والد محمد",
                gender = "Male",
                MosqueId = mosqueId,
                SaturdayHalqeId = halqaId,
                EnrollmentDate = DateTime.UtcNow,
                created_at = DateTime.UtcNow,
                JoinedAt = DateTime.UtcNow,
                status = 1,
                age = 35,
                score = 0,
                progressEntrys = new List<ProgressEntry>(),
                Exams = new List<Exam>(),
                Attendances = new List<Attendance>(),
                Children = new List<Student>()
            };

            // ربط الطالب بولي الأمر
            student.ParentId = parent.Id;
            parent.Children.Add(student);

            // حفظ المستخدمين في قاعدة البيانات (مع تشفير كلمة المرور)
            await CreateUserIfNotExists(userManager, teacher, "Password123!");
            await CreateUserIfNotExists(userManager, student, "Password123!");
            await CreateUserIfNotExists(userManager, parent, "Password123!");

            // 5. إنشاء منشورات (Posts)
            var post1 = new Post
            {
                Id = Guid.NewGuid(),
                MosqueId = mosqueId, // ✅ استخدام الـ ID الحقيقي
                title = "جدول الامتحانات الشهري",
                body = "السلام عليكم، نرفق لكم جدول الامتحانات...",
                imageUrl = "https://via.placeholder.com/150",
                created_at = DateTime.UtcNow,
                PosInteractions = new List<PosInteraction>()
            };

            var post2 = new Post
            {
                Id = Guid.NewGuid(),
                MosqueId = mosqueId, // ✅ استخدام الـ ID الحقيقي
                title = "رحلة المسجد الشهرية",
                body = "تعلن إدارة المسجد عن رحلة ترفيهية...",
                imageUrl = "https://via.placeholder.com/150",
                created_at = DateTime.UtcNow,
                PosInteractions = new List<PosInteraction>()
            };

            await context.Posts.AddRangeAsync(post1, post2);
            await context.SaveChangesAsync();

            // 6. إنشاء تفاعلات (PosInteractions)
            var interaction1 = new PosInteraction
            {
                Id = Guid.NewGuid(),
                PostId = post1.Id, // ✅ ربط بالمنشور الأول
                UserId = student.Id, // ✅ ربط بالطالب
                date = DateTime.UtcNow
            };

            var interaction2 = new PosInteraction
            {
                Id = Guid.NewGuid(),
                PostId = post2.Id, // ✅ ربط بالمنشور الثاني
                UserId = teacher.Id, // ✅ ربط بالمعلم
                date = DateTime.UtcNow
            };

            await context.PosInteractions.AddRangeAsync(interaction1, interaction2);
            await context.SaveChangesAsync();
        }

        // ================= [دالة مساعدة] =================
        private static async Task CreateUserIfNotExists(UserManager<User> userManager, User user, string password)
        {
            var existingUser = await userManager.FindByIdAsync(user.Id.ToString());
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create user {user.UserName}: {errors}");
                }
            }
        }
    }
}