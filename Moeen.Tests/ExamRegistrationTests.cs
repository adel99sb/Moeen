using Microsoft.EntityFrameworkCore;
using Moeen.Api.Application.Services;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.ExamCommand;

namespace Moeen.Tests;

public class ExamRegistrationTests
{
    [Fact]
    public async Task RegisterExam_SavesWithoutFabricatedTeacherExamForeignKey()
    {
        await using var context = CreateContext();
        var seed = await SeedAsync(context, studentStatus: 0);
        var service = new ExamCommandService(new UnitOfWork(context));

        var result = await service.RegisterExamAsync(new RegisterExamRequest
        {
            StudentId = seed.StudentId,
            TeacherId = seed.TeacherId,
            JuzFrom = 1,
            JuzTo = 2,
            Score = 92,
            Mark = 92,
            Notes = "أداء جيد"
        });

        Assert.True(result.Success, result.Message);
        var exam = await context.Exams.SingleAsync();
        Assert.Equal(seed.StudentId, exam.StudentId);
        Assert.Equal(seed.TeacherId, exam.TeacherId);
        Assert.Null(exam.TeacherExamId);
    }

    [Fact]
    public async Task RegisterExam_RejectsInactiveStudentWithoutSaving()
    {
        await using var context = CreateContext();
        var seed = await SeedAsync(context, studentStatus: 1);
        var service = new ExamCommandService(new UnitOfWork(context));

        var result = await service.RegisterExamAsync(new RegisterExamRequest
        {
            StudentId = seed.StudentId,
            TeacherId = seed.TeacherId,
            JuzFrom = 1,
            JuzTo = 2,
            Score = 80,
            Mark = 80,
            Notes = string.Empty
        });

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("غير نشط", result.Message);
        Assert.Empty(context.Exams);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<(Guid StudentId, Guid TeacherId)> SeedAsync(
        AppDbContext context,
        int studentStatus)
    {
        var now = DateTime.UtcNow;
        var mosque = new Mosque
        {
            Id = Guid.NewGuid(),
            name = "مسجد الاختبار",
            address = "عنوان اختباري",
            contact_phone = string.Empty,
            Description = string.Empty
        };
        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            MosqueId = mosque.Id,
            Mosque = mosque,
            UserName = "exam-registration-teacher",
            name = "معلم الاختبار",
            role = 3,
            status = 0,
            created_at = now,
            JoinedAt = now,
            assigned_at = string.Empty,
            Bio = string.Empty
        };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            MosqueId = mosque.Id,
            Mosque = mosque,
            SaturdayHalqeId = Guid.NewGuid(),
            UserName = "exam-registration-student",
            name = "طالب الاختبار",
            role = 2,
            status = studentStatus,
            created_at = now,
            JoinedAt = now,
            EnrollmentDate = now,
            gender = "ذكر"
        };

        context.Mosques.Add(mosque);
        context.Teachers.Add(teacher);
        context.Students.Add(student);
        await context.SaveChangesAsync();

        return (student.Id, teacher.Id);
    }
}
