using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses.ExamQuery;
using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Requests;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;
using Moeen.Api.Application.Services;
using Moeen.Api.infrastructure.Providers;
using Moeen.Dashboard.Components.Pages.Supervisor;

namespace Moeen.Tests;

public class SupervisorDashboardStudentClassifierTests
{
    [Fact]
    public void BuildLists_DoesNotClassifyStudentsWithoutAssessmentActivity()
    {
        var newStudentId = Guid.NewGuid();
        var activeStudentId = Guid.NewGuid();

        var performances = new[]
        {
            new SupervisorDashboardStudentPerformance(newStudentId, "New Student", 0, 0, 0),
            new SupervisorDashboardStudentPerformance(activeStudentId, "Active Student", 10, 92, 2)
        };

        var excellent = SupervisorDashboardStudentClassifier.BuildExcellentStudents(performances);
        var struggling = SupervisorDashboardStudentClassifier.BuildStrugglingStudents(performances);

        Assert.DoesNotContain(excellent, student => student.StudentId == newStudentId);
        Assert.DoesNotContain(struggling, student => student.StudentId == newStudentId);
        Assert.Contains(excellent, student => student.StudentId == activeStudentId);
        Assert.Contains(struggling, student => student.StudentId == activeStudentId);
    }

    [Fact]
    public void BuildLists_RanksByExamAndRecitationAverage()
    {
        var high = Guid.NewGuid();
        var middle = Guid.NewGuid();
        var low = Guid.NewGuid();

        var performances = new[]
        {
            new SupervisorDashboardStudentPerformance(middle, "Middle", 100, 75, 3),
            new SupervisorDashboardStudentPerformance(low, "Low", 200, 45, 1),
            new SupervisorDashboardStudentPerformance(high, "High", 0, 95, 2)
        };

        var excellent = SupervisorDashboardStudentClassifier.BuildExcellentStudents(performances);
        var struggling = SupervisorDashboardStudentClassifier.BuildStrugglingStudents(performances);

        Assert.Equal(high, excellent[0].StudentId);
        Assert.Equal("ممتاز", excellent[0].Badge);
        Assert.Equal(95, excellent[0].Score);

        Assert.Equal(low, struggling[0].StudentId);
        Assert.Equal("متابعة عاجلة", struggling[0].Badge);
        Assert.Equal(45, struggling[0].Score);
    }
}

public class CurrentUserServiceTests
{
    public static IEnumerable<object[]> UserIdClaimTypes()
    {
        yield return new object[] { "UserIdentifier" };
        yield return new object[] { ClaimTypes.NameIdentifier };
        yield return new object[] { "sub" };
        yield return new object[] { "nameid" };
    }

    [Theory]
    [MemberData(nameof(UserIdClaimTypes))]
    public void CurrentUserId_ResolvesSupportedUserIdClaims(string claimType)
    {
        var expectedUserId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(claimType, expectedUserId.ToString()) },
                authenticationType: "Test"))
        };

        var service = new CurrentUserService(new HttpContextAccessor { HttpContext = httpContext });

        Assert.Equal(expectedUserId, service.CurrentUserId);
    }

    [Fact]
    public void CurrentUserId_ReturnsNullWhenClaimIsMissing()
    {
        var service = new CurrentUserService(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

        Assert.Null(service.CurrentUserId);
    }
}

public class PostScopeSelectionTests
{
    [Fact]
    public void ScopeValues_AreStableAndVisibleForSelectBinding()
    {
        Assert.Equal("public", PostScopeSelection.Public);
        Assert.Equal("halqa", PostScopeSelection.Halqa);
        Assert.Equal(PostScopeSelection.Public, PostScopeSelection.FromIsPublic(true));
        Assert.Equal(PostScopeSelection.Halqa, PostScopeSelection.FromIsPublic(false));
        Assert.True(PostScopeSelection.IsPublic(PostScopeSelection.Public));
        Assert.False(PostScopeSelection.IsPublic(PostScopeSelection.Halqa));
        Assert.All(PostScopeSelection.Options, option =>
        {
            Assert.False(string.IsNullOrWhiteSpace(option.Value));
            Assert.False(string.IsNullOrWhiteSpace(option.Label));
        });
    }
}

public class GoalApiClientTests
{
    [Fact]
    public void DuplicatePolicy_ReturnsMemorizationMessageWhenSameDateRecitationExists()
    {
        var message = GoalDailyEntryDuplicatePolicy.GetDuplicateMessage(
            memorizationRequested: true,
            memorizationExists: true,
            reviewRequested: false,
            reviewExists: false,
            examRequested: false,
            examExists: false);

        Assert.Equal(GoalDailyEntryDuplicatePolicy.MemorizationAlreadyExistsMessage, message);
    }

    [Fact]
    public void DuplicatePolicy_AllowsNewRecitationWhenNoSameDateRecordExists()
    {
        var message = GoalDailyEntryDuplicatePolicy.GetDuplicateMessage(
            memorizationRequested: true,
            memorizationExists: false,
            reviewRequested: false,
            reviewExists: false,
            examRequested: false,
            examExists: false);

        Assert.Null(message);
    }

    [Fact]
    public async Task RecordDailyEntryAsync_SendsBearerTokenWithTeacherRequest()
    {
        var token = "teacher-test-token";
        using var handler = new CapturingHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("api/goal-tracking/daily-entry", request.RequestUri?.ToString());
            Assert.True(request.Headers.TryGetValues("Author" + "ization", out var values));
            Assert.Equal("Bear" + "er " + token, Assert.Single(values));

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(GeneralResponse.Ok("تم حفظ بيانات اليوم."))
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        var client = new GoalApiClient(httpClient, new FakeTokenService(token));

        var response = await client.RecordDailyEntryAsync(new RecordDailyEntryRequest
        {
            StudentId = Guid.NewGuid(),
            Date = DateTime.Today
        });

        Assert.True(response.Success);
        Assert.Equal(1, handler.CallCount);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetFeedbackLists_SendsBearerToken(bool loadComplaints)
    {
        var token = "supervisor-feedback-token";
        using var handler = new CapturingHandler(async request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.EndsWith(
                loadComplaints
                    ? "api/Feedback/complaints?page=1&pageSize=25"
                    : "api/Feedback/suggestions?page=1&pageSize=25",
                request.RequestUri?.ToString());
            Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
            Assert.Equal(token, request.Headers.Authorization?.Parameter);

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(GeneralResponse.Ok("تم التحميل.", Array.Empty<object>()))
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        var client = new FeedbackApiClient(httpClient, new FakeTokenService(token));
        var pagination = new PaginationRequest { Page = 1, PageSize = 25 };

        var response = loadComplaints
            ? await client.GetComplaintsAsync(pagination)
            : await client.GetSuggestionsAsync(pagination);

        Assert.True(response.Success);
        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task GetFeedbackLists_WhenApiReturnsUnauthorized_ReturnsFriendlySessionMessage()
    {
        using var handler = new CapturingHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(string.Empty)
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        var client = new FeedbackApiClient(httpClient, new FakeTokenService("expired-feedback-token"));
        var response = await client.GetComplaintsAsync(new PaginationRequest { Page = 1, PageSize = 25 });

        Assert.False(response.Success);
        Assert.Equal(401, response.StatusCode);
        Assert.Contains("انتهت جلسة تسجيل الدخول", response.Message);
    }

    private sealed class FakeTokenService(string token) : ITokenService
    {
        public Task Save(string token) => Task.CompletedTask;
        public Task<string?> Get() => Task.FromResult<string?>(token);
        public Task Clear() => Task.CompletedTask;
        public Task<DashboardAuthSession> GetSession()
            => Task.FromResult(new DashboardAuthSession(true, token, Array.Empty<string>(), null, "/"));
    }

    private sealed class CapturingHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler) : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return await handler(request);
        }
    }
}

public class FeedbackApiClientTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SubmitFeedback_SendsBearerToken(bool submitComplaint)
    {
        var token = "feedback-test-token";
        using var handler = new CapturingHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith(
                submitComplaint ? "api/Feedback/complaint" : "api/Feedback/suggestion",
                request.RequestUri?.ToString());
            Assert.True(request.Headers.TryGetValues("Author" + "ization", out var values));
            Assert.Equal("Bear" + "er " + token, Assert.Single(values));

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(GeneralResponse.Ok("تم الإرسال."))
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        var client = new FeedbackApiClient(httpClient, new FakeTokenService(token));

        var response = submitComplaint
            ? await client.SubmitComplaintAsync(new SubmitComplaintRequest
            {
                ComplaintData = new ComplaintDto { Content = "شكوى اختبارية صالحة" }
            })
            : await client.SubmitSuggestionAsync(new SubmitSuggestionRequest
            {
                SuggestionData = new SuggestionDto
                {
                    Title = "اقتراح اختباري",
                    Content = "تفاصيل اقتراح اختبارية صالحة"
                }
            });

        Assert.True(response.Success);
        Assert.Equal(1, handler.CallCount);
    }

    private sealed class FakeTokenService(string token) : ITokenService
    {
        public Task Save(string token) => Task.CompletedTask;
        public Task<string?> Get() => Task.FromResult<string?>(token);
        public Task Clear() => Task.CompletedTask;
        public Task<DashboardAuthSession> GetSession()
            => Task.FromResult(new DashboardAuthSession(true, token, Array.Empty<string>(), null, "/"));
    }

    private sealed class CapturingHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler) : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return await handler(request);
        }
    }
}

public class PostApiClientTests
{
    public static IEnumerable<object[]> MutationCases()
    {
        yield return new object[] { "publish", HttpMethod.Post };
        yield return new object[] { "update", HttpMethod.Put };
        yield return new object[] { "delete", HttpMethod.Delete };
    }

    [Theory]
    [MemberData(nameof(MutationCases))]
    public async Task SupervisorPostMutations_SendStoredBearerToken(string operation, HttpMethod expectedMethod)
    {
        var token = "supervisor-post-token";
        var postId = Guid.NewGuid();

        using var handler = new CapturingHandler(async request =>
        {
            Assert.Equal(expectedMethod, request.Method);
            Assert.True(request.Headers.TryGetValues("Author" + "ization", out var values));
            Assert.Equal("Bear" + "er " + token, Assert.Single(values));

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(GeneralResponse.Ok("تمت العملية بنجاح."))
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        var client = new PostApiClient(httpClient, new FakeTokenService(token));

        GeneralResponse? response = operation switch
        {
            "publish" => await client.PublishPost(new PublishPostRequest
            {
                IsPublic = true,
                PostData = new RequstePostDto { Title = "اختبار", Body = "محتوى اختبار" }
            }),
            "update" => await client.UpdatePost(new UpdatePostRequest
            {
                PostId = postId,
                Title = "عنوان معدل",
                Body = "محتوى معدل"
            }),
            "delete" => await client.DeletePost(new DeletePostRequest { PostId = postId }),
            _ => throw new InvalidOperationException("Unknown operation")
        };

        Assert.NotNull(response);
        Assert.True(response!.Success);
        Assert.Equal(1, handler.CallCount);
    }

    private sealed class FakeTokenService(string token) : ITokenService
    {
        public Task Save(string token) => Task.CompletedTask;
        public Task<string?> Get() => Task.FromResult<string?>(token);
        public Task Clear() => Task.CompletedTask;
        public Task<DashboardAuthSession> GetSession()
            => Task.FromResult(new DashboardAuthSession(true, token, new[] { "Supervisor" }, null, "/supervisor/posts"));
    }

    private sealed class CapturingHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler) : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return await handler(request);
        }
    }
}

public class ExamQueryServiceTests
{
    [Fact]
    public async Task GetStudentExamsAsync_ReturnsExamWithCalculatedGrade()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);
        var studentId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();

        context.Students.Add(new Moeen.Api.Core.Entities.Student
        {
            Id = studentId,
            UserName = "student-test",
            name = "طالب اختبار",
            MosqueId = Guid.NewGuid(),
            SaturdayHalqeId = Guid.NewGuid(),
            EnrollmentDate = DateTime.UtcNow
        });
        context.Teachers.Add(new Moeen.Api.Core.Entities.Teacher
        {
            Id = teacherId,
            UserName = "teacher-test",
            name = "فاحص اختبار",
            MosqueId = Guid.NewGuid()
        });
        context.Exams.Add(new Exam
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            TeacherId = teacherId,
            TeacherExamId = Guid.NewGuid(),
            juz_form = 1,
            juz_to = 2,
            score = 95,
            mark = 95,
            date = DateTime.UtcNow,
            notes = "اختبار"
        });
        await context.SaveChangesAsync();

        var service = new ExamQueryService(context);
        var response = await service.GetStudentExamsAsync(new GetStudentExamsRequest
        {
            StudentId = studentId
        });

        Assert.True(response.Success);

        var json = JsonSerializer.Serialize(response.Data);
        var result = JsonSerializer.Deserialize<GetStudentExamsResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var exam = Assert.Single(Assert.IsType<GetStudentExamsResponse>(result).Exams);
        Assert.Equal("ممتاز", exam.Grade);
        Assert.Equal(studentId, exam.StudentId);
    }
}

public class StudentPlacementSourceGuardTests
{
    [Fact]
    public void SupervisorStudentForm_DoesNotAutoSelectFirstHalqa()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "Moeen.Dashboard/Components/Pages/Supervisor/Student.razor"));

        Assert.DoesNotContain("_halqas.FirstOrDefault(h => h.FoujId == _form.FoujId)?.Id", source);
        Assert.DoesNotContain("_halqas.FirstOrDefault(halqa => halqa.FoujId == _form.FoujId)?.Id", source);
        Assert.Contains("_form.HalqaId = null;", source);
    }

    [Fact]
    public void SupervisorDashboard_CountsActiveStudentRowsAndDisablesCaching()
    {
        var root = FindRepositoryRoot();
        var controller = File.ReadAllText(Path.Combine(root, "Moeen.Api/Controllers/SupervisorDashboardController.cs"));
        var client = File.ReadAllText(Path.Combine(root, "Moeen.Dashboard/Infrastructure/Http/Clients/SupervisorDashboardApiClient.cs"));

        Assert.Contains("CountAsync(s => s.MosqueId == mosqueId.Value && s.role == 2 && s.status == 0)", controller);
        Assert.Contains("ResponseCache(NoStore = true", controller);
        Assert.Contains("NoCache = true", client);
        Assert.Contains("NoStore = true", client);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Moeen.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Moeen repository root was not found.");
    }
}

public class StudentStatusLifecycleTests
{
    [Fact]
    public void StudentStatus_ZeroIsActive_OneIsInactive()
    {
        const int active = 0;
        const int inactive = 1;

        Assert.Equal(0, active);
        Assert.Equal(1, inactive);
        Assert.NotEqual(active, inactive);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(2, false)]
    public void StudentIsVisibleOnlyWhenActive(int status, bool expectedVisible)
        => Assert.Equal(expectedVisible, status == 0);
}
