using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;

namespace Moeen.Api.Infrastructure.Data
{
    public static partial class AppSeeder
    {
        private static readonly Guid DevelopmentMobileFoujId = Guid.Parse("88888888-8888-8888-8888-888888888888");
        private static readonly Guid DevelopmentMobileHalqaId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        private static readonly Guid DevelopmentMobileSaturdayHalqaId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid DevelopmentMobileSaturdayLessonId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        private static readonly Guid DevelopmentMobileBookIntroId = Guid.Parse("c0000000-0000-0000-0000-000000000001");
        private static readonly Guid DevelopmentMobileBookTajweedId = Guid.Parse("c0000000-0000-0000-0000-000000000002");
        private static readonly Guid DevelopmentMobileBookHadithId = Guid.Parse("c0000000-0000-0000-0000-000000000003");
        private static readonly Guid DevelopmentMobileBookAdabId = Guid.Parse("c0000000-0000-0000-0000-000000000004");
        private static readonly Guid DevelopmentMobilePublicPostId = Guid.Parse("d0000000-0000-0000-0000-000000000001");
        private static readonly Guid DevelopmentMobileHalqaPostId = Guid.Parse("d0000000-0000-0000-0000-000000000002");
        private static readonly Guid DevelopmentMobileReminderPostId = Guid.Parse("d0000000-0000-0000-0000-000000000003");

        private static async Task SeedDevelopmentMobileAccountsAsync(AppDbContext context, UserManager<User> userManager, string seedSecret)
        {
            var student = await context.Students.FirstOrDefaultAsync(x => x.Id == DevelopmentStudentId)
                          ?? await context.Students.FirstOrDefaultAsync(x => x.Email == DevelopmentStudentEmail);

            if (student == null)
            {
                student = new Student
                {
                    Id = DevelopmentStudentId,
                    UserName = DevelopmentStudentEmail,
                    Email = DevelopmentStudentEmail,
                    EmailConfirmed = true,
                    name = "Dev Mobile Student",
                    gender = "Male",
                    font_size = 16,
                    role = DevelopmentStudentRole,
                    theme = "light",
                    created_at = DateTime.UtcNow,
                    JoinedAt = DateTime.UtcNow,
                    age = 12,
                    EnrollmentDate = DateTime.UtcNow,
                    status = 0,
                    score = 0,
                    MosqueId = DevelopmentMosqueId,
                    SaturdayHalqeId = Guid.Empty,
                    SaturdayHalqaId = null,
                    HalqaId = null,
                    complaints = new List<Complaint>(),
                    PosInteractions = new List<PosInteraction>(),
                    progressEntrys = new List<ProgressEntry>(),
                    Exams = new List<Exam>(),
                    Attendances = new List<Attendance>(),
                    Children = new List<Student>()
                };

                var result = await userManager.CreateAsync(student, seedSecret);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed mobile student: {errors}");
                }
            }
            else
            {
                student.UserName = DevelopmentStudentEmail;
                student.Email = DevelopmentStudentEmail;
                student.EmailConfirmed = true;
                student.name = "Dev Mobile Student";
                student.gender = "Male";
                student.font_size = student.font_size == 0 ? 16 : student.font_size;
                student.role = DevelopmentStudentRole;
                student.theme = string.IsNullOrWhiteSpace(student.theme) ? "light" : student.theme;
                student.MosqueId = DevelopmentMosqueId;
                student.age = student.age == 0 ? 12 : student.age;
                student.status = 0;
                student.EnrollmentDate = student.EnrollmentDate == default ? DateTime.UtcNow : student.EnrollmentDate;
                student.JoinedAt = student.JoinedAt == default ? DateTime.UtcNow : student.JoinedAt;
                student.created_at = student.created_at == default ? DateTime.UtcNow : student.created_at;
            }

            await EnsureDevelopmentUserHasOnlyRoleAsync(userManager, student, Roles.Student, "mobile student");

            var parent = await context.Students.FirstOrDefaultAsync(x => x.Id == DevelopmentParentId)
                         ?? await context.Students.FirstOrDefaultAsync(x => x.Email == DevelopmentParentEmail);

            if (parent == null)
            {
                parent = new Student
                {
                    Id = DevelopmentParentId,
                    UserName = DevelopmentParentEmail,
                    Email = DevelopmentParentEmail,
                    EmailConfirmed = true,
                    name = "Dev Mobile Parent",
                    gender = "Male",
                    font_size = 16,
                    role = DevelopmentParentRole,
                    theme = "Father",
                    created_at = DateTime.UtcNow,
                    JoinedAt = DateTime.UtcNow,
                    age = 0,
                    EnrollmentDate = DateTime.UtcNow,
                    status = 1,
                    score = 0,
                    MosqueId = student.MosqueId,
                    SaturdayHalqeId = student.SaturdayHalqeId,
                    SaturdayHalqaId = student.SaturdayHalqaId,
                    HalqaId = null,
                    complaints = new List<Complaint>(),
                    PosInteractions = new List<PosInteraction>(),
                    progressEntrys = new List<ProgressEntry>(),
                    Exams = new List<Exam>(),
                    Attendances = new List<Attendance>(),
                    Children = new List<Student> { student }
                };

                var result = await userManager.CreateAsync(parent, seedSecret);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed mobile parent: {errors}");
                }
            }
            else
            {
                parent.UserName = DevelopmentParentEmail;
                parent.Email = DevelopmentParentEmail;
                parent.EmailConfirmed = true;
                parent.name = "Dev Mobile Parent";
                parent.gender = "Male";
                parent.font_size = parent.font_size == 0 ? 16 : parent.font_size;
                parent.role = DevelopmentParentRole;
                parent.theme = string.IsNullOrWhiteSpace(parent.theme) ? "Father" : parent.theme;
                parent.MosqueId = student.MosqueId;
                parent.SaturdayHalqeId = student.SaturdayHalqeId;
                parent.SaturdayHalqaId = student.SaturdayHalqaId;
                parent.JoinedAt = parent.JoinedAt == default ? DateTime.UtcNow : parent.JoinedAt;
                parent.created_at = parent.created_at == default ? DateTime.UtcNow : parent.created_at;
            }

            student.ParentId = parent.Id;
            await context.SaveChangesAsync();
            await EnsureDevelopmentUserHasOnlyRoleAsync(userManager, parent, Roles.ParentSudent, "mobile parent");
            await SeedDevelopmentMobileProgressDataAsync(context, student);
            await SeedDevelopmentMobileLibraryDataAsync(context);
            await SeedDevelopmentMobilePostDataAsync(context);
        }

        private static async Task SeedDevelopmentMobileProgressDataAsync(AppDbContext context, Student student)
        {
            await EnsureDevelopmentMobileHalqaAsync(context);

            student.HalqaId = DevelopmentMobileHalqaId;
            student.MosqueId = DevelopmentMosqueId;
            student.score = 92;
            student.status = 0;
            student.EnrollmentDate = student.EnrollmentDate == default ? DateTime.UtcNow.AddMonths(-4) : student.EnrollmentDate;

            var today = DateTime.UtcNow.Date;
            var entries = new[]
            {
                new ProgressEntry
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    StudentId = student.Id,
                    HalqaId = DevelopmentMobileHalqaId,
                    TeacherId = DevelopmentTeacherId,
                    JuzNumber = 1,
                    PageNumber = 1,
                    MemorizedUntil = 3,
                    NextTarget = 4,
                    LevelScore = 9,
                    Date = today.AddDays(-2),
                    IsDeleted = false
                },
                new ProgressEntry
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    StudentId = student.Id,
                    HalqaId = DevelopmentMobileHalqaId,
                    TeacherId = DevelopmentTeacherId,
                    JuzNumber = 1,
                    PageNumber = 4,
                    MemorizedUntil = 6,
                    NextTarget = 7,
                    LevelScore = 8,
                    Date = today.AddDays(-1),
                    IsDeleted = false
                },
                new ProgressEntry
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    StudentId = student.Id,
                    HalqaId = DevelopmentMobileHalqaId,
                    TeacherId = DevelopmentTeacherId,
                    JuzNumber = 1,
                    PageNumber = 1,
                    MemorizedUntil = 6,
                    NextTarget = 0,
                    LevelScore = 10,
                    Date = today.AddDays(-1),
                    IsDeleted = false
                },
                new ProgressEntry
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    StudentId = student.Id,
                    HalqaId = DevelopmentMobileHalqaId,
                    TeacherId = DevelopmentTeacherId,
                    JuzNumber = 2,
                    PageNumber = 21,
                    MemorizedUntil = 23,
                    NextTarget = 24,
                    LevelScore = 7,
                    Date = today.AddDays(-8),
                    IsDeleted = false
                },
                new ProgressEntry
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    StudentId = student.Id,
                    HalqaId = DevelopmentMobileHalqaId,
                    TeacherId = DevelopmentTeacherId,
                    JuzNumber = 2,
                    PageNumber = 21,
                    MemorizedUntil = 23,
                    NextTarget = 0,
                    LevelScore = 9,
                    Date = today.AddDays(-10),
                    IsDeleted = false
                },
                new ProgressEntry
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    StudentId = student.Id,
                    HalqaId = DevelopmentMobileHalqaId,
                    TeacherId = DevelopmentTeacherId,
                    JuzNumber = 3,
                    PageNumber = 41,
                    MemorizedUntil = 43,
                    NextTarget = -1,
                    LevelScore = 6,
                    Date = today.AddDays(-15),
                    IsDeleted = false
                },
                new ProgressEntry
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                    StudentId = student.Id,
                    HalqaId = DevelopmentMobileHalqaId,
                    TeacherId = DevelopmentTeacherId,
                    JuzNumber = 3,
                    PageNumber = 44,
                    MemorizedUntil = 46,
                    NextTarget = 47,
                    LevelScore = 0,
                    Date = today,
                    IsDeleted = false
                }
            };

            foreach (var entry in entries)
            {
                var existing = await context.ProgressEntries.FirstOrDefaultAsync(x => x.Id == entry.Id);
                if (existing == null)
                {
                    await context.ProgressEntries.AddAsync(entry);
                    continue;
                }

                existing.StudentId = entry.StudentId;
                existing.HalqaId = entry.HalqaId;
                existing.TeacherId = entry.TeacherId;
                existing.JuzNumber = entry.JuzNumber;
                existing.PageNumber = entry.PageNumber;
                existing.MemorizedUntil = entry.MemorizedUntil;
                existing.NextTarget = entry.NextTarget;
                existing.LevelScore = entry.LevelScore;
                existing.Date = entry.Date;
                existing.IsDeleted = false;
                existing.DeletedAt = null;
            }

            await context.SaveChangesAsync();
        }

        private static async Task EnsureDevelopmentMobileHalqaAsync(AppDbContext context)
        {
            var fouj = await context.Foujs.FirstOrDefaultAsync(x => x.Id == DevelopmentMobileFoujId);
            if (fouj == null)
            {
                fouj = new Fouj
                {
                    Id = DevelopmentMobileFoujId,
                    name = "الفوج التجريبي لتطبيق الطالب",
                    start_time = DateTime.UtcNow.Date.AddHours(8),
                    End_time = new TimeSpan(10, 0, 0),
                    MosqueId = DevelopmentMosqueId,
                    Halqas = new List<Halqa>(),
                    ExamTeacherHalqas = new List<ExamTeacherHalqa>()
                };
                await context.Foujs.AddAsync(fouj);
            }
            else
            {
                fouj.name = "الفوج التجريبي لتطبيق الطالب";
                fouj.MosqueId = DevelopmentMosqueId;
                fouj.start_time = fouj.start_time == default ? DateTime.UtcNow.Date.AddHours(8) : fouj.start_time;
                fouj.End_time = fouj.End_time == default ? new TimeSpan(10, 0, 0) : fouj.End_time;
            }

            var halqa = await context.Halqas.FirstOrDefaultAsync(x => x.Id == DevelopmentMobileHalqaId);
            if (halqa == null)
            {
                halqa = new Halqa
                {
                    Id = DevelopmentMobileHalqaId,
                    FoujId = DevelopmentMobileFoujId,
                    TeacherId = DevelopmentTeacherId,
                    Name = "حلقة الطالب التجريبية",
                    Type = "MobileSeed",
                    ProgressEntries = new List<ProgressEntry>(),
                    HalqeSessions = new List<HalqaSession>(),
                    ExamTeacherHalqas = new List<ExamTeacherHalqa>(),
                    Students = new List<Student>()
                };
                await context.Halqas.AddAsync(halqa);
            }
            else
            {
                halqa.FoujId = DevelopmentMobileFoujId;
                halqa.TeacherId = DevelopmentTeacherId;
                halqa.Name = "حلقة الطالب التجريبية";
                halqa.Type = "MobileSeed";
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedDevelopmentMobileLibraryDataAsync(AppDbContext context)
        {
            var saturdayHalqa = await context.SaturdayHalqes.FirstOrDefaultAsync(x => x.Id == DevelopmentMobileSaturdayHalqaId);
            if (saturdayHalqa == null)
            {
                saturdayHalqa = new SaturdayHalqa
                {
                    Id = DevelopmentMobileSaturdayHalqaId,
                    TeacherId = DevelopmentTeacherId,
                    name = "حلقة مكتبة التطبيق التجريبية",
                    age_min = 7,
                    age_max = 18,
                    MosqueId = DevelopmentMosqueId,
                    SaturdayLessons = new List<SaturdayLesson>(),
                    Students = new List<Student>()
                };
                await context.SaturdayHalqes.AddAsync(saturdayHalqa);
            }
            else
            {
                saturdayHalqa.TeacherId = DevelopmentTeacherId;
                saturdayHalqa.name = "حلقة مكتبة التطبيق التجريبية";
                saturdayHalqa.age_min = 7;
                saturdayHalqa.age_max = 18;
            }

            var saturdayLessons = context.Set<SaturdayLesson>();
            var lesson = await saturdayLessons.FirstOrDefaultAsync(x => x.Id == DevelopmentMobileSaturdayLessonId);
            if (lesson == null)
            {
                lesson = new SaturdayLesson
                {
                    Id = DevelopmentMobileSaturdayLessonId,
                    SaturdayHalqeId = DevelopmentMobileSaturdayHalqaId,
                    HalqaId = DevelopmentMobileHalqaId,
                    TeacherId = DevelopmentTeacherId,
                    lesson_number = 1,
                    start_time = new TimeSpan(8, 0, 0),
                    end_time = new TimeSpan(10, 0, 0),
                    attendances = new List<Attendance>(),
                    PdfFiles = new List<PdfFile>()
                };
                await saturdayLessons.AddAsync(lesson);
            }
            else
            {
                lesson.SaturdayHalqeId = DevelopmentMobileSaturdayHalqaId;
                lesson.HalqaId = DevelopmentMobileHalqaId;
                lesson.TeacherId = DevelopmentTeacherId;
                lesson.lesson_number = 1;
                lesson.start_time = lesson.start_time == default ? new TimeSpan(8, 0, 0) : lesson.start_time;
                lesson.end_time = lesson.end_time == default ? new TimeSpan(10, 0, 0) : lesson.end_time;
            }

            var books = new[]
            {
                new PdfFile
                {
                    Id = DevelopmentMobileBookIntroId,
                    MosqueId = DevelopmentMosqueId,
                    SaturdayLessonId = DevelopmentMobileSaturdayLessonId,
                    FileUrl = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                    title = "مدخل إلى حفظ القرآن الكريم",
                    description = "كتاب تجريبي يشرح خطة الحفظ اليومية وطريقة المتابعة داخل تطبيق معين.",
                    uploaded_by = "مكتبة معين",
                    created_at = DateTime.UtcNow.AddMonths(-3)
                },
                new PdfFile
                {
                    Id = DevelopmentMobileBookTajweedId,
                    MosqueId = DevelopmentMosqueId,
                    SaturdayLessonId = DevelopmentMobileSaturdayLessonId,
                    FileUrl = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                    title = "أساسيات التجويد للمبتدئين",
                    description = "ملخص مبسط لأحكام التجويد الأساسية مع أمثلة مناسبة للطلاب.",
                    uploaded_by = "مكتبة معين",
                    created_at = DateTime.UtcNow.AddMonths(-2)
                },
                new PdfFile
                {
                    Id = DevelopmentMobileBookHadithId,
                    MosqueId = DevelopmentMosqueId,
                    SaturdayLessonId = DevelopmentMobileSaturdayLessonId,
                    FileUrl = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                    title = "أحاديث مختارة للحفظ",
                    description = "مجموعة أحاديث قصيرة مناسبة للحفظ والمراجعة الأسبوعية.",
                    uploaded_by = "مكتبة معين",
                    created_at = DateTime.UtcNow.AddMonths(-1)
                },
                new PdfFile
                {
                    Id = DevelopmentMobileBookAdabId,
                    MosqueId = DevelopmentMosqueId,
                    SaturdayLessonId = DevelopmentMobileSaturdayLessonId,
                    FileUrl = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                    title = "آداب طالب القرآن",
                    description = "إرشادات تربوية مختصرة تساعد الطالب على تنظيم وقته واحترام مجلس القرآن.",
                    uploaded_by = "مكتبة معين",
                    created_at = DateTime.UtcNow.AddDays(-10)
                }
            };

            foreach (var book in books)
            {
                var existing = await context.PdfFiles.FirstOrDefaultAsync(x => x.Id == book.Id);
                if (existing == null)
                {
                    await context.PdfFiles.AddAsync(book);
                    continue;
                }

                existing.MosqueId = book.MosqueId;
                existing.SaturdayLessonId = book.SaturdayLessonId;
                existing.FileUrl = book.FileUrl;
                existing.title = book.title;
                existing.description = book.description;
                existing.uploaded_by = book.uploaded_by;
                existing.created_at = book.created_at;
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedDevelopmentMobilePostDataAsync(AppDbContext context)
        {
            var posts = new[]
            {
                new Post
                {
                    Id = DevelopmentMobilePublicPostId,
                    MosqueId = DevelopmentMosqueId,
                    HalqaId = null,
                    title = "إعلان عام لطلاب المسجد",
                    body = "يرجى الالتزام بمواعيد الحضور وإحضار المصحف ودفتر المتابعة لكل درس.",
                    imageUrl = string.Empty,
                    created_at = DateTime.UtcNow.AddDays(-3),
                    PosInteractions = new List<PosInteraction>()
                },
                new Post
                {
                    Id = DevelopmentMobileHalqaPostId,
                    MosqueId = DevelopmentMosqueId,
                    HalqaId = DevelopmentMobileHalqaId,
                    title = "واجب حلقة الطالب التجريبية",
                    body = "واجب هذا الأسبوع: مراجعة الصفحات السابقة وتجهيز التسميع القادم حسب الخطة.",
                    imageUrl = string.Empty,
                    created_at = DateTime.UtcNow.AddDays(-1),
                    PosInteractions = new List<PosInteraction>()
                },
                new Post
                {
                    Id = DevelopmentMobileReminderPostId,
                    MosqueId = DevelopmentMosqueId,
                    HalqaId = null,
                    title = "تذكير بمتابعة التقدم",
                    body = "يمكنك متابعة تقدمك من صفحة التقدم والاطلاع على الواجبات اليومية من الصفحة الرئيسية.",
                    imageUrl = string.Empty,
                    created_at = DateTime.UtcNow,
                    PosInteractions = new List<PosInteraction>()
                }
            };

            foreach (var post in posts)
            {
                var existing = await context.Posts.FirstOrDefaultAsync(x => x.Id == post.Id);
                if (existing == null)
                {
                    await context.Posts.AddAsync(post);
                    continue;
                }

                existing.MosqueId = post.MosqueId;
                existing.HalqaId = post.HalqaId;
                existing.title = post.title;
                existing.body = post.body;
                existing.imageUrl = post.imageUrl;
                existing.created_at = post.created_at;
            }

            await context.SaveChangesAsync();
        }

    }
}

