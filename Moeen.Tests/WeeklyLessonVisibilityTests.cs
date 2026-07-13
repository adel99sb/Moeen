using Microsoft.EntityFrameworkCore;
using Moeen.Api.Application.Services;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Responses.LessonManagement;

namespace Moeen.Tests;

public class WeeklyLessonVisibilityTests
{
    [Fact]
    public async Task SupervisorList_IncludesLessonWithoutAnyAssignments()
    {
        await using var context = CreateContext();
        var mosque = new Mosque
        {
            Id = Guid.NewGuid(),
            name = "Test Mosque",
            address = "Test Address"
        };
        var supervisor = new Supervisor
        {
            Id = Guid.NewGuid(),
            UserName = "supervisor@test.local",
            Email = "supervisor@test.local",
            name = "Test Supervisor",
            role = 4,
            MosqueId = mosque.Id,
            Mosque = mosque,
            created_at = DateTime.UtcNow,
            JoinedAt = DateTime.UtcNow
        };
        var lesson = new WeeklyLesson
        {
            Id = Guid.NewGuid(),
            Title = "فقه",
            Description = "درس بلا حلقة أو وقت",
            CreatedAt = DateTime.UtcNow
        };

        context.AddRange(mosque, supervisor, lesson);
        await context.SaveChangesAsync();

        var service = new LessonManagementService(
            context,
            new FakeCurrentUserService(supervisor.Id));

        var response = await service.GetManagedWeeklyLessonsAsync();

        Assert.True(response.Success);
        var lessons = Assert.IsType<List<WeeklyLessonManagementDto>>(response.Data);
        var returnedLesson = Assert.Single(lessons);
        Assert.Equal(lesson.Id, returnedLesson.Id);
        Assert.Equal("فقه", returnedLesson.Title);
        Assert.Equal(0, returnedLesson.AssignmentsCount);
        Assert.Empty(returnedLesson.Assignments);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new AppDbContext(options);
    }

    private sealed class FakeCurrentUserService(Guid userId) : ICurrentUserService
    {
        public Guid? CurrentUserId => userId;
        public string CurrentUserName => "supervisor";
        public bool? IsActived => true;
        public bool? IsAdmin => false;
        public bool IsInRole(string roleName)
            => string.Equals(roleName, "Supervisor", StringComparison.OrdinalIgnoreCase);
        public string GetBaseUrl(string relativePath) => relativePath;
    }
}
