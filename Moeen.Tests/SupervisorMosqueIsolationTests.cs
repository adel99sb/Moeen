using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moeen.Api.Application.Services;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Requests.Fouj;
using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses.ContentSharing;
using Moeen.Shared.Responses.Enrollment;
using Moeen.Shared.Responses.Fouj;
using Moeen.Shared.Responses.Mosuq;

namespace Moeen.Tests;

public class SupervisorMosqueIsolationTests
{
    [Fact]
    public async Task Supervisor_OnlySeesMembersAndMosqueFromAssignedMosque()
    {
        await using var db = CreateContext();
        var mosqueA = new Mosque { Id = Guid.NewGuid(), name = "Mosque A", address = "A" };
        var mosqueB = new Mosque { Id = Guid.NewGuid(), name = "Mosque B", address = "B" };
        var supervisorA = Supervisor(Guid.NewGuid(), "Supervisor A", mosqueA);
        var supervisorB = Supervisor(Guid.NewGuid(), "Supervisor B", mosqueB);
        var studentA = Student(Guid.NewGuid(), "Student A", mosqueA);
        var studentB = Student(Guid.NewGuid(), "Student B", mosqueB);
        var teacherA = Teacher(Guid.NewGuid(), "Teacher A", mosqueA);
        var teacherB = Teacher(Guid.NewGuid(), "Teacher B", mosqueB);

        db.AddRange(mosqueA, mosqueB, supervisorA, supervisorB, studentA, studentB, teacherA, teacherB);
        await db.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(supervisorA.Id);
        var enrollment = new EnrollmentService(db, null!, null!, currentUser);
        var mosqueService = new MosquService(db, currentUser);

        var students = Assert.IsType<List<StudentDto>>((await enrollment.GetAllStudentsAsync(new GetAllStudentsRequest
        {
            PageNumber = 1,
            PageSize = 20,
            MosqueId = mosqueB.Id
        })).Data);
        Assert.Single(students);
        Assert.Equal(studentA.Id, students[0].Id);

        var teachers = Assert.IsType<List<TeacherDto>>((await enrollment.GetAllTeachersAsync(new GetAllTeachersRequest
        {
            PageNumber = 1,
            PageSize = 20,
            MosqueId = mosqueB.Id
        })).Data);
        Assert.Single(teachers);
        Assert.Equal(teacherA.Id, teachers[0].Id);

        var supervisors = Assert.IsType<List<MemberProfileDto>>((await enrollment.GetAllSupervisorsAsync(new GetAllSupervisorsRequest
        {
            PageNumber = 1,
            PageSize = 20
        })).Data);
        Assert.Single(supervisors);
        Assert.Equal(supervisorA.Id, supervisors[0].Id);

        var mosques = Assert.IsType<List<MosquDto>>((await mosqueService.GetAllMosqus(new GetAllMosqusRequest
        {
            Page = 1,
            PageSize = 20
        })).Data);
        Assert.Single(mosques);
        Assert.Equal(mosqueA.Id, mosques[0].Id);
    }

    [Fact]
    public async Task SupervisorFoujList_UsesAssignedMosqueEvenWhenAdminClaimIsAlsoPresent()
    {
        await using var db = CreateContext();
        var mosqueA = new Mosque { Id = Guid.NewGuid(), name = "Mosque A", address = "A" };
        var mosqueB = new Mosque { Id = Guid.NewGuid(), name = "Mosque B", address = "B" };
        var supervisorA = Supervisor(Guid.NewGuid(), "Supervisor A", mosqueA);
        var foujA = new Fouj
        {
            Id = Guid.NewGuid(),
            name = "Fouj A",
            MosqueId = mosqueA.Id,
            Mosque = mosqueA,
            start_time = DateTime.Today.AddHours(8),
            End_time = TimeSpan.FromHours(10)
        };
        var foujB = new Fouj
        {
            Id = Guid.NewGuid(),
            name = "Fouj B",
            MosqueId = mosqueB.Id,
            Mosque = mosqueB,
            start_time = DateTime.Today.AddHours(9),
            End_time = TimeSpan.FromHours(11)
        };

        db.AddRange(mosqueA, mosqueB, supervisorA, foujA, foujB);
        await db.SaveChangesAsync();

        var service = new FoujService(db, new FakeCurrentUserService(supervisorA.Id));
        var response = await service.GetAllFoujsAsync(new GetAllFoujsRequest { MosqueId = mosqueB.Id });
        var foujs = Assert.IsType<List<FoujDto>>(response.Data);

        Assert.Single(foujs);
        Assert.Equal(foujA.Id, foujs[0].Id);
        Assert.Equal(mosqueA.Id, foujs[0].MosqueId);
    }

    [Fact]
    public async Task Supervisors_FromDifferentMosques_SeeTheSameGlobalPostList()
    {
        await using var db = CreateContext();
        var mosqueA = new Mosque { Id = Guid.NewGuid(), name = "Mosque A", address = "A" };
        var mosqueB = new Mosque { Id = Guid.NewGuid(), name = "Mosque B", address = "B" };
        var supervisorA = Supervisor(Guid.NewGuid(), "Supervisor A", mosqueA);
        var supervisorB = Supervisor(Guid.NewGuid(), "Supervisor B", mosqueB);
        var postA = new Post
        {
            Id = Guid.NewGuid(),
            MosqueId = mosqueA.Id,
            Mosque = mosqueA,
            title = "Post A",
            body = "From Mosque A",
            created_at = DateTime.UtcNow.AddMinutes(-1)
        };
        var postB = new Post
        {
            Id = Guid.NewGuid(),
            MosqueId = mosqueB.Id,
            Mosque = mosqueB,
            title = "Post B",
            body = "From Mosque B",
            created_at = DateTime.UtcNow
        };

        db.AddRange(mosqueA, mosqueB, supervisorA, supervisorB, postA, postB);
        await db.SaveChangesAsync();

        var serviceA = new ContentSharingService(new UnitOfWork(db), new FakeCurrentUserService(supervisorA.Id), new FakeFileService());
        var serviceB = new ContentSharingService(new UnitOfWork(db), new FakeCurrentUserService(supervisorB.Id), new FakeFileService());

        var postsForA = Assert.IsType<List<PostDto>>((await serviceA.GetAllPostsAsync()).Data);
        var postsForB = Assert.IsType<List<PostDto>>((await serviceB.GetAllPostsAsync()).Data);

        Assert.Equal(2, postsForA.Count);
        Assert.Equal(2, postsForB.Count);
        Assert.Equal(postsForA.Select(p => p.Id), postsForB.Select(p => p.Id));
        Assert.Contains(postsForA, p => p.MosqueId == mosqueA.Id && p.MosqueName == mosqueA.name);
        Assert.Contains(postsForA, p => p.MosqueId == mosqueB.Id && p.MosqueName == mosqueB.name);
    }

    [Fact]
    public async Task Supervisor_CannotReadOrUpdateMemberFromAnotherMosque()
    {
        await using var db = CreateContext();
        var mosqueA = new Mosque { Id = Guid.NewGuid(), name = "Mosque A", address = "A" };
        var mosqueB = new Mosque { Id = Guid.NewGuid(), name = "Mosque B", address = "B" };
        var supervisorA = Supervisor(Guid.NewGuid(), "Supervisor A", mosqueA);
        var studentB = Student(Guid.NewGuid(), "Student B", mosqueB);

        db.AddRange(mosqueA, mosqueB, supervisorA, studentB);
        await db.SaveChangesAsync();

        var service = new EnrollmentService(db, null!, null!, new FakeCurrentUserService(supervisorA.Id));

        var profile = await service.GetMemberProfileAsync(new GetMemberProfileRequest { MemberId = studentB.Id.ToString() });
        Assert.False(profile.Success);
        Assert.Equal(401, profile.StatusCode);

        var update = await service.UpdateStudentInfoAsync(new UpdateStudentInfoRequest
        {
            StudentId = studentB.Id,
            Score = 99
        });
        Assert.False(update.Success);
        Assert.Equal(401, update.StatusCode);

        Assert.Equal(0, (await db.Students.FindAsync(studentB.Id))!.score);
    }

    [Fact]
    public async Task AnonymousMemberLists_FailClosedInsteadOfReturningAllMosques()
    {
        await using var db = CreateContext();
        var mosqueA = new Mosque { Id = Guid.NewGuid(), name = "Mosque A", address = "A" };
        var mosqueB = new Mosque { Id = Guid.NewGuid(), name = "Mosque B", address = "B" };
        db.AddRange(
            mosqueA,
            mosqueB,
            Student(Guid.NewGuid(), "Student A", mosqueA),
            Student(Guid.NewGuid(), "Student B", mosqueB),
            Teacher(Guid.NewGuid(), "Teacher A", mosqueA),
            Teacher(Guid.NewGuid(), "Teacher B", mosqueB));
        await db.SaveChangesAsync();

        var service = new EnrollmentService(db, null!, null!, new FakeCurrentUserService(null));

        var students = await service.GetAllStudentsAsync(new GetAllStudentsRequest { PageNumber = 1, PageSize = 20 });
        var teachers = await service.GetAllTeachersAsync(new GetAllTeachersRequest { PageNumber = 1, PageSize = 20 });

        Assert.False(students.Success);
        Assert.Equal(401, students.StatusCode);
        Assert.False(teachers.Success);
        Assert.Equal(401, teachers.StatusCode);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    private static Supervisor Supervisor(Guid id, string name, Mosque mosque) => new()
    {
        Id = id,
        name = name,
        UserName = $"{id:N}@test.local",
        Email = $"{id:N}@test.local",
        role = 4,
        MosqueId = mosque.Id,
        Mosque = mosque,
        created_at = DateTime.UtcNow,
        JoinedAt = DateTime.UtcNow
    };

    private static Student Student(Guid id, string name, Mosque mosque) => new()
    {
        Id = id,
        name = name,
        UserName = $"{id:N}@test.local",
        Email = $"{id:N}@test.local",
        role = 2,
        status = 0,
        score = 0,
        age = 12,
        MosqueId = mosque.Id,
        Mosque = mosque,
        created_at = DateTime.UtcNow,
        JoinedAt = DateTime.UtcNow,
        EnrollmentDate = DateTime.UtcNow
    };

    private static Teacher Teacher(Guid id, string name, Mosque mosque) => new()
    {
        Id = id,
        name = name,
        UserName = $"{id:N}@test.local",
        Email = $"{id:N}@test.local",
        role = 1,
        status = 0,
        MosqueId = mosque.Id,
        Mosque = mosque,
        created_at = DateTime.UtcNow,
        JoinedAt = DateTime.UtcNow
    };

    private sealed class FakeFileService : IFileService
    {
        public Task<string> UploadFileAsync(FilePathType fileType, Guid ownerId, string fileName, byte[] fileData)
            => Task.FromResult(fileName);

        public Task<bool> DeleteFileAsync(string filePath)
            => Task.FromResult(true);

        public Task<string> GetFileUrlAsync(string filePath)
            => Task.FromResult(filePath);
    }

    private sealed class FakeCurrentUserService(Guid? userId) : ICurrentUserService
    {
        public Guid? CurrentUserId => userId;
        public string CurrentUserName => "supervisor";
        public bool? IsActived => true;
        public bool? IsAdmin => true;
        public bool IsInRole(string roleName) => true;
        public string GetBaseUrl(string relativePath) => relativePath;
    }
}
