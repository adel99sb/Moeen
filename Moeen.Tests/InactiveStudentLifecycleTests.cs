using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Moeen.Api.Application.Services;
using Moeen.Api.Controllers;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Requests.HalqaQuery;
using Moeen.Shared.Requests.Reporting;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Enrollment;
using Moeen.Shared.Responses.HalqaQuery;
using Moeen.Shared.Responses.Mobile;
using Moeen.Shared.Responses.OwnerDashboard;
using Moeen.Shared.Responses.Reporting;
using Moeen.Shared.Responses.SupervisorDashboard;

namespace Moeen.Tests;

public class InactiveStudentLifecycleIntegrationTests
{
    [Fact]
    public async Task ParentAndStudentMobileViews_HideInactiveStudentUntilReactivated()
    {
        await using var db = CreateContext();
        var seed = await SeedAsync(db);

        var parentDashboard = new ParentMobileDashboardService(db);
        var dashboard = AssertOkData<ParentDashboardResponse>(await parentDashboard.GetDashboardAsync(seed.ParentId, null));
        Assert.Single(dashboard.Children);
        Assert.Equal(seed.ActiveStudentId, dashboard.Children[0].StudentId);
        Assert.Equal(401, (await parentDashboard.GetDashboardAsync(seed.ParentId, seed.InactiveStudentId)).StatusCode);

        var parentProfile = AssertOkData<ParentProfileResponse>(await new ParentMobileProfileService(db).GetProfileAsync(seed.ParentId));
        Assert.Equal(1, parentProfile.ChildrenCount);
        Assert.Equal(seed.ActiveScore, parentProfile.TotalChildrenPoints);
        Assert.DoesNotContain(parentProfile.Children, c => c.StudentId == seed.InactiveStudentId);

        var parentProgressSvc = new ParentMobileProgressService(db, new StudentMobileProgressService(db));
        var parentProgress = AssertOkData<ParentProgressResponse>(await parentProgressSvc.GetProgressAsync(seed.ParentId, null, null, null, null));
        Assert.Equal(seed.ActiveStudentId, parentProgress.SelectedStudentId);
        Assert.Equal(401, (await parentProgressSvc.GetProgressAsync(seed.ParentId, seed.InactiveStudentId, null, null, null)).StatusCode);

        Assert.Equal(404, (await new StudentMobileDashboardService(db).GetDashboardAsync(seed.InactiveStudentId)).StatusCode);
        Assert.Equal(404, (await new StudentMobileProfileService(db).GetProfileAsync(seed.InactiveStudentId)).StatusCode);
        Assert.Equal(404, (await new StudentMobileProgressService(db).GetProgressAsync(seed.InactiveStudentId, null, null, null)).StatusCode);
        Assert.Equal(404, (await new StudentMobilePostService(db, new FakeFileService()).GetPostsAsync(seed.InactiveStudentId)).StatusCode);

        var inactive = await db.Students.FindAsync(seed.InactiveStudentId);
        inactive!.status = 0;
        await db.SaveChangesAsync();
        var reactivated = AssertOkData<ParentDashboardResponse>(await parentDashboard.GetDashboardAsync(seed.ParentId, null));
        Assert.Contains(reactivated.Children, c => c.StudentId == seed.InactiveStudentId);
    }


    [Fact]
    public async Task GoalDailyEntry_RejectsInactiveStudentButKeepsStoredHistoricalData()
    {
        await using var db = CreateContext();
        var seed = await SeedAsync(db);
        var service = new GoalService(db, new FakeCurrentUserService(seed.TeacherId));
        var inactiveBefore = await db.ProgressEntries.CountAsync(p => p.StudentId == seed.InactiveStudentId);
        var activeBefore = await db.ProgressEntries.CountAsync(p => p.StudentId == seed.ActiveStudentId);

        var inactiveResult = await service.RecordDailyEntryAsync(NewMemorizationRequest(seed.InactiveStudentId, seed.Today));
        Assert.False(inactiveResult.Success);
        Assert.Equal(400, inactiveResult.StatusCode);
        Assert.Contains("غير نشط", inactiveResult.Message);
        Assert.Equal(inactiveBefore, await db.ProgressEntries.CountAsync(p => p.StudentId == seed.InactiveStudentId));
        Assert.True(await db.Students.AnyAsync(s => s.Id == seed.InactiveStudentId));

        var activeResult = await service.RecordDailyEntryAsync(NewMemorizationRequest(seed.ActiveStudentId, seed.Today));
        Assert.True(activeResult.Success, activeResult.Message);
        Assert.Equal(activeBefore + 1, await db.ProgressEntries.CountAsync(p => p.StudentId == seed.ActiveStudentId));
    }

    [Fact]
    public async Task HalqaQueries_ExcludeInactiveStudentsFromListsCountsStatisticsAndAttendanceReports()
    {
        await using var db = CreateContext();
        var seed = await SeedAsync(db);
        var service = new HalqaQueryService(db, new FakeCurrentUserService(seed.SupervisorId));

        var allHalqas = await service.GetAllHalqasAsync(seed.MosqueId);
        Assert.Single(allHalqas);
        Assert.Equal(1, allHalqas[0].StudentsCount);

        var assignments = await service.GetAssignmentStudentsAsync(seed.HalqaId);
        Assert.Contains(assignments, s => s.Id == seed.ActiveStudentId);
        Assert.DoesNotContain(assignments, s => s.Id == seed.InactiveStudentId);

        var students = await service.GetHalqaStudentsAsync(new GetHalqaStudentsRequest
        {
            HalqaId = seed.HalqaId,
            Filter = new StudentFilterDto { PageNumber = 1, PageSize = 10 }
        });
        Assert.Equal(1, students.TotalCount);
        Assert.Equal(seed.ActiveStudentId, Assert.Single(students.Students).Id);

        Assert.Equal(1, (await service.GetHalqaStudentsCountAsync(new GetHalqaStudentsCountRequest { HalqaId = seed.HalqaId })).Count);

        var stats = await service.GetHalqaStatisticsAsync(new GetHalqaStatisticsRequest
        {
            HalqaId = seed.HalqaId,
            FromDate = seed.Today.AddDays(-1),
            ToDate = seed.Today.AddDays(1)
        });
        Assert.Equal(1, stats.StudentsCount);
        Assert.Equal(1, stats.ActiveStudentsCount);
        Assert.Equal(seed.ActiveMemorizedUntil, stats.AverageMemorizationProgress);
        Assert.Equal(0, stats.AttendanceRate);

        var attendance = await service.GetHalqaAttendanceReportAsync(new GetHalqaAttendanceReportRequest
        {
            HalqaId = seed.HalqaId,
            FromDate = seed.Today.AddDays(-1),
            ToDate = seed.Today.AddDays(1)
        });
        Assert.Equal(0, Assert.Single(attendance.DailyRecords).PresentCount);
    }


    [Fact]
    public async Task DashboardsAndReports_ExcludeInactiveStudentsFromTotalsPerformanceAndRankings()
    {
        await using var db = CreateContext();
        var seed = await SeedAsync(db);

        var owner = new OwnerDashboardController(db, NullLogger<OwnerDashboardController>.Instance);
        owner.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var ownerResult = await owner.GetOverview();
        var ownerResponse = Assert.IsType<OwnerDashboardResponse>(Assert.IsType<OkObjectResult>(ownerResult.Result).Value);
        Assert.Equal(1, ownerResponse.TotalStudents);
        Assert.Equal(1, Assert.Single(ownerResponse.MosqueSummaries).Students);

        var supervisor = new SupervisorDashboardController(db, NullLogger<SupervisorDashboardController>.Instance);
        supervisor.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("UserIdentifier", seed.SupervisorId.ToString()) }, "Test"))
            }
        };
        var supervisorResult = await supervisor.GetOverview();
        var supervisorResponse = Assert.IsType<SupervisorDashboardResponse>(Assert.IsType<OkObjectResult>(supervisorResult.Result).Value);
        Assert.Equal(1, supervisorResponse.TotalStudents);
        Assert.Equal(1, Assert.Single(supervisorResponse.HalqaPerformance).Tests);
        Assert.Equal(1, supervisorResponse.HalqaPerformance[0].Recitations);
        Assert.Equal(0, supervisorResponse.HalqaPerformance[0].Attendance);
        Assert.DoesNotContain(supervisorResponse.ExcellentStudents, s => s.StudentId == seed.InactiveStudentId);
        Assert.DoesNotContain(supervisorResponse.StrugglingStudents, s => s.StudentId == seed.InactiveStudentId);

        var report = await new ReportingService(db).GetGeneralPerformanceIndicatorsAsync(new GetGeneralPerformanceIndicatorsRequest());
        var reportData = AssertOkData<GeneralPerformanceIndicatorsDto>(report);
        Assert.Equal(1, reportData.TotalStudents);
        Assert.DoesNotContain(reportData.TopStudents, s => s.StudentId == seed.InactiveStudentId);
        Assert.DoesNotContain(reportData.LowStudents, s => s.StudentId == seed.InactiveStudentId);
    }

    [Fact]
    public async Task SupervisorManagement_CanSeeInactiveStudentButParentAndStatsCannotUntilReactivated()
    {
        await using var db = CreateContext();
        var seed = await SeedAsync(db);
        var service = new EnrollmentService(db, null!, null!);

        var supervisorList = AssertOkData<List<StudentDto>>(await service.GetAllStudentsAsync(new GetAllStudentsRequest
        {
            PageNumber = 1,
            PageSize = 10,
            MosqueId = seed.MosqueId
        }));
        Assert.Contains(supervisorList, s => s.Id == seed.ActiveStudentId && s.Status == 0);
        Assert.Contains(supervisorList, s => s.Id == seed.InactiveStudentId && s.Status == 1);

        var childrenBefore = AssertOkData<List<StudentDto>>(await service.GetChildrenByParentAsync(new GetChildrenByParentRequest
        {
            ParentId = seed.ParentId
        }));
        Assert.Single(childrenBefore);
        Assert.DoesNotContain(childrenBefore, s => s.Id == seed.InactiveStudentId);

        var statsBefore = await service.GetMemberStatisticsAsync(new GetMemberStatisticsRequest { MosqueId = seed.MosqueId });
        Assert.Equal(1, ReadInt(statsBefore.Data, "StudentsCount"));
        Assert.Equal(1, ReadInt(statsBefore.Data, "ActiveStudents"));
        Assert.Equal(0, ReadInt(statsBefore.Data, "InactiveStudents"));

        var update = await service.UpdateStudentInfoAsync(new UpdateStudentInfoRequest
        {
            StudentId = seed.InactiveStudentId,
            Status = 0
        });
        Assert.True(update.Success, update.Message);

        var childrenAfter = AssertOkData<List<StudentDto>>(await service.GetChildrenByParentAsync(new GetChildrenByParentRequest
        {
            ParentId = seed.ParentId
        }));
        Assert.Contains(childrenAfter, s => s.Id == seed.InactiveStudentId && s.Status == 0);

        var statsAfter = await service.GetMemberStatisticsAsync(new GetMemberStatisticsRequest { MosqueId = seed.MosqueId });
        Assert.Equal(2, ReadInt(statsAfter.Data, "StudentsCount"));
        Assert.Equal(2, ReadInt(statsAfter.Data, "ActiveStudents"));
    }

    [Fact]
    public async Task ReportingTimeline_ReturnsNotFoundForInactiveStudentAndRestoresHistoricalRecordsAfterActivation()
    {
        await using var db = CreateContext();
        var seed = await SeedAsync(db);
        var service = new ReportingService(db);
        var request = new GetStudentProgressTimelineRequest
        {
            StudentId = seed.InactiveStudentId,
            FromDate = seed.Today.AddDays(-2),
            ToDate = seed.Today.AddDays(1),
            PageNumber = 1,
            PageSize = 20
        };

        var inactiveResult = await service.GetStudentProgressTimelineAsync(request);
        Assert.False(inactiveResult.Success);
        Assert.Equal(404, inactiveResult.StatusCode);

        var inactive = await db.Students.FindAsync(seed.InactiveStudentId);
        inactive!.status = 0;
        await db.SaveChangesAsync();

        var activeResult = await service.GetStudentProgressTimelineAsync(request);
        var timeline = AssertOkData<StudentProgressReportDto>(activeResult);
        Assert.Equal(999, timeline.Summary.TotalPoints);
        Assert.Equal(1, timeline.Summary.ExamsCount);
        Assert.Equal(1, timeline.Summary.MemorizationCount);
        Assert.Contains(timeline.Records, r => r.Type == "Exam" && r.Score == 100);
        Assert.Contains(timeline.Records, r => r.Type == "Memorization" && r.Points == 100);
    }

    [Fact]
    public void SourceGuards_EnsureLoginBlocksInactiveButSupervisorManagementCanReactivate()
    {
        var root = FindRepositoryRoot();
        var userService = File.ReadAllText(Path.Combine(root, "Moeen.Api/Application/Services/UserService.cs"));
        var enrollmentService = File.ReadAllText(Path.Combine(root, "Moeen.Api/Application/Services/EnrollmentService.cs"));

        Assert.Contains("studentAccount.role == 2 && studentAccount.status != 0", userService);
        Assert.True(userService.IndexOf("studentAccount.status != 0", StringComparison.Ordinal) < userService.IndexOf("GenerateJwtToken", StringComparison.Ordinal));
        Assert.Contains("teacherAccount != null && teacherAccount.status != 0", userService);
        Assert.True(userService.IndexOf("teacherAccount.status != 0", StringComparison.Ordinal) < userService.IndexOf("GenerateJwtToken", StringComparison.Ordinal));
        Assert.Contains(".Where(s => s.role == StudentRole);", enrollmentService);
        Assert.Contains("linkedStudentIds.Contains(s.Id) || s.ParentId == request.ParentId", enrollmentService);
        Assert.Contains("&& s.status == 0", enrollmentService);
        Assert.Contains("var studentsQuery = _context.Students.Where(s => s.role == StudentRole && s.status == 0).AsQueryable();", enrollmentService);
        Assert.Contains("var teachersQuery = _context.Teachers.Where(t => t.status == ActiveStatus).AsQueryable();", enrollmentService);
        Assert.Contains("DetachTeacherAssignmentsAsync(teacher.Id)", enrollmentService);
    }


    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<Seed> SeedAsync(AppDbContext db)
    {
        var today = DateTime.UtcNow.Date;
        var mosque = new Mosque { Id = Guid.NewGuid(), name = "Test Mosque", address = "Address" };
        var teacher = new Teacher { Id = Guid.NewGuid(), name = "Teacher", Email = "teacher@test.local", UserName = "teacher@test.local", role = 1, MosqueId = mosque.Id, Mosque = mosque, created_at = today, JoinedAt = today };
        var supervisor = new Supervisor { Id = Guid.NewGuid(), name = "Supervisor", Email = "supervisor@test.local", UserName = "supervisor@test.local", role = 4, MosqueId = mosque.Id, Mosque = mosque, created_at = today, JoinedAt = today };
        var fouj = new Fouj { Id = Guid.NewGuid(), name = "Fouj", MosqueId = mosque.Id, Mosque = mosque };
        var halqa = new Halqa { Id = Guid.NewGuid(), Name = "Halqa", Type = "regular", FoujId = fouj.Id, Fouj = fouj, TeacherId = teacher.Id, Teacher = teacher };
        var parent = Student(Guid.NewGuid(), "Parent", 3, 0, 0, mosque, halqa, today);
        var active = Student(Guid.NewGuid(), "Active", 2, 0, 12, mosque, halqa, today, parent.Id);
        var inactive = Student(Guid.NewGuid(), "Inactive", 2, 1, 999, mosque, halqa, today, parent.Id);
        var session = new HalqaSession { Id = Guid.NewGuid(), HalqaId = halqa.Id, Halqa = halqa, date = today, start_time = TimeSpan.FromHours(8), end_time = TimeSpan.FromHours(9) };
        const int activeMemorizedUntil = 33;

        db.AddRange(
            mosque, teacher, supervisor, fouj, halqa, parent, active, inactive, session,
            new Post { Id = Guid.NewGuid(), MosqueId = mosque.Id, Mosque = mosque, title = "Post", body = "Body", created_at = today },
            Progress(active, teacher, halqa, today.AddDays(-1), 85, activeMemorizedUntil),
            Progress(inactive, teacher, halqa, today.AddDays(-1), 100, 604),
            Exam(active, teacher, today.AddDays(-1), 80),
            Exam(inactive, teacher, today.AddDays(-1), 100),
            new Attendance { Id = Guid.NewGuid(), StudentId = inactive.Id, Student = inactive, TeacherId = teacher.Id, Teacher = teacher, HalqeSessionId = session.Id, HalqeSession = session, Status = AttendanceStatus.Present });

        await db.SaveChangesAsync();
        return new Seed(mosque.Id, teacher.Id, supervisor.Id, halqa.Id, parent.Id, active.Id, inactive.Id, active.score, activeMemorizedUntil, today);
    }

    private static Student Student(Guid id, string name, int role, int status, int score, Mosque mosque, Halqa halqa, DateTime today, Guid? parentId = null) => new()
    {
        Id = id,
        name = name,
        UserName = $"{id:N}@test.local",
        Email = $"{id:N}@test.local",
        role = role,
        status = status,
        score = score,
        age = 12,
        gender = "ذكر",
        MosqueId = mosque.Id,
        Mosque = mosque,
        HalqaId = halqa.Id,
        Halqa = halqa,
        ParentId = parentId,
        EnrollmentDate = today.AddMonths(-1),
        JoinedAt = today.AddMonths(-1),
        created_at = today.AddMonths(-1)
    };

    private static ProgressEntry Progress(Student student, Teacher teacher, Halqa halqa, DateTime date, int score, int memorizedUntil) => new()
    {
        Id = Guid.NewGuid(),
        StudentId = student.Id,
        Student = student,
        TeacherId = teacher.Id,
        Teacher = teacher,
        HalqaId = halqa.Id,
        Halqa = halqa,
        Date = date,
        JuzNumber = 1,
        PageNumber = 1,
        MemorizedUntil = memorizedUntil,
        NextTarget = 1,
        LevelScore = score,
        IsDeleted = false
    };

    private static Exam Exam(Student student, Teacher teacher, DateTime date, int score) => new()
    {
        Id = Guid.NewGuid(),
        StudentId = student.Id,
        Student = student,
        TeacherId = teacher.Id,
        Teacher = teacher,
        date = date,
        juz_form = 1,
        juz_to = 1,
        mark = score,
        score = score
    };

    private static RecordDailyEntryRequest NewMemorizationRequest(Guid studentId, DateTime date) => new()
    {
        StudentId = studentId,
        Date = date,
        Memorization = new MemorizationEntryDto { JuzNumber = 1, FromPage = 2, ToPage = 3, Grade = Grade.Excellent }
    };

    private static T AssertOkData<T>(GeneralResponse response)
    {
        Assert.True(response.Success, response.Message);
        Assert.Equal(200, response.StatusCode);
        return Assert.IsType<T>(response.Data);
    }

    private static int ReadInt(object? source, string propertyName)
    {
        Assert.NotNull(source);
        var property = source!.GetType().GetProperty(propertyName);
        Assert.NotNull(property);
        var value = property!.GetValue(source);
        return value switch
        {
            int intValue => intValue,
            JsonElement json when json.ValueKind == JsonValueKind.Number => json.GetInt32(),
            _ => throw new Xunit.Sdk.XunitException($"Property '{propertyName}' was not an int. Actual value: {value}")
        };
    }

    private static string FindRepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Moeen.sln")))
            dir = dir.Parent;
        Assert.NotNull(dir);
        return dir!.FullName;
    }

    private sealed record Seed(Guid MosqueId, Guid TeacherId, Guid SupervisorId, Guid HalqaId, Guid ParentId, Guid ActiveStudentId, Guid InactiveStudentId, int ActiveScore, int ActiveMemorizedUntil, DateTime Today);

    private sealed class FakeCurrentUserService(Guid userId) : ICurrentUserService
    {
        public Guid? CurrentUserId => userId;
        public string CurrentUserName => "test-user";
        public bool? IsActived => true;
        public bool? IsAdmin => false;
        public bool IsInRole(string roleName) => true;
        public string GetBaseUrl(string relativePath) => relativePath;
    }

    private sealed class FakeFileService : IFileService
    {
        public Task<string> UploadFileAsync(FilePathType fileType, Guid ownerId, string fileName, byte[] fileData) => Task.FromResult(fileName);
        public Task<bool> DeleteFileAsync(string filePath) => Task.FromResult(true);
        public Task<string> GetFileUrlAsync(string filePath) => Task.FromResult(filePath);
    }
}
