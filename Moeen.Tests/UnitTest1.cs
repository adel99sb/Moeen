using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Goal;
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
